using UnityEngine;

[ExecuteAlways]
public class ArenaBuilder : MonoBehaviour
{
    [Header("Arena size (world units)")]
    [Min(1f)] public float width = 6f;
    [Min(1f)] public float length = 12f;

    [Header("Floor")]
    [Min(0.01f)] public float floorThickness = 0.2f;

    [Header("Walls")]
    [Min(0.05f)] public float wallThickness = 0.4f;
    [Min(0.2f)] public float wallHeight = 0.6f;

    [Header("Spawn")]
    public Transform spawnPoint;
    [Min(0f)] public float spawnMarginFromBottom = 0.6f;

    [Header("References")]
    public Transform floor;
    public Transform wallLeft;
    public Transform wallRight;
    public Transform wallTop;

    [Header("Start bar (visual only)")]
    public Transform startBar;
    [Min(0.01f)] public float startBarThickness = 0.05f;
    [Min(0.01f)] public float startBarHeight = 0.05f;
    [Min(0f)] public float startBarYOffset = 0.01f;
    [Min(-5f)] public float startBarZOffset = 0f;

    [Header("Start zone (trigger)")]
    public BoxCollider startZoneTrigger;
    [Min(0.01f)] public float startZoneHeight = 1.0f; 
    [Min(0f)] public float startZoneYOffset = 0.5f;


    void OnValidate() => Rebuild();

    public void Rebuild()
    {
        if (!floor || !wallLeft || !wallRight || !wallTop) return;

        float halfW = width * 0.5f;
        float halfL = length * 0.5f;

        floor.localScale = new Vector3(width, floorThickness, length);
        floor.localPosition = new Vector3(0f, -floorThickness * 0.5f, 0f);

        float wallZ = length + wallThickness;
        wallLeft.localScale = new Vector3(wallThickness, wallHeight, wallZ);
        wallRight.localScale = new Vector3(wallThickness, wallHeight, wallZ);

        float wallCenterZ = wallThickness * 0.5f;
        wallLeft.localPosition = new Vector3(-halfW - wallThickness * 0.5f, wallHeight * 0.5f, wallCenterZ);
        wallRight.localPosition = new Vector3(halfW + wallThickness * 0.5f, wallHeight * 0.5f, wallCenterZ);

        float wallX = width + wallThickness * 2f;
        wallTop.localScale = new Vector3(wallX, wallHeight, wallThickness);
        wallTop.localPosition = new Vector3(0f, wallHeight * 0.5f, halfL + wallThickness * 0.5f);

        if (spawnPoint != null)
        {
            float z = -halfL + spawnMarginFromBottom;
            z = Mathf.Clamp(z, -halfL + 0.05f, halfL - 0.05f);

            spawnPoint.localPosition = new Vector3(0f, 0f, z);
        }

        if (startBar != null)
        {
            float halfWl = width * 0.5f;
            float halfLl = length * 0.5f;


            float baseZ = -halfLl + spawnMarginFromBottom;
            baseZ = Mathf.Clamp(baseZ, -halfLl + 0.05f, halfLl - 0.05f);

            float finalZ = baseZ + startBarZOffset;

            startBar.localScale = new Vector3(width, startBarHeight, startBarThickness);
            startBar.localPosition = new Vector3(0f, startBarYOffset, finalZ);
        }

        if (startZoneTrigger != null)
        {
            startZoneTrigger.isTrigger = true;

            float halfL2 = length * 0.5f;

            float bottomZ = -halfL2;

            float lineCenterZ = startBar != null
                ? startBar.localPosition.z
                : (-halfL2 + spawnMarginFromBottom + startBarZOffset);

            float halfLineZ = 0f;
            if (startBar != null)
                halfLineZ = Mathf.Abs(startBar.localScale.z) * 0.5f;
            else
                halfLineZ = startBarThickness * 0.5f;

            float zoneTopZ = lineCenterZ + halfLineZ;

            float zoneLength = Mathf.Max(0.01f, zoneTopZ - bottomZ);
            float centerZ = bottomZ + zoneLength * 0.5f;

            startZoneTrigger.size = new Vector3(width, startZoneHeight, zoneLength);
            startZoneTrigger.center = new Vector3(0f, startZoneYOffset, centerZ);
        }

    }

    public float GetFloorTopYWorld() { return transform.TransformPoint(Vector3.zero).y; }
}
