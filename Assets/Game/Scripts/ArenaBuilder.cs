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

    [Header("References")]
    public Transform floor;
    public Transform wallLeft;
    public Transform wallRight;
    public Transform wallTop;

    void OnValidate() => Rebuild();

    public void Rebuild()
    {
        if (!floor || !wallLeft || !wallRight || !wallTop) return;

        float halfW = width * 0.5f;
        float halfL = length * 0.5f;

        floor.localScale = new Vector3(width, floorThickness, length);
        floor.localPosition = new Vector3(0f, -floorThickness * 0.5f, 0f);

        float wallZ = length + wallThickness * 2f;
        wallLeft.localScale = new Vector3(wallThickness, wallHeight, wallZ);
        wallRight.localScale = new Vector3(wallThickness, wallHeight, wallZ);

        wallLeft.localPosition = new Vector3(-halfW - wallThickness * 0.5f, wallHeight * 0.5f, 0f);
        wallRight.localPosition = new Vector3(halfW + wallThickness * 0.5f, wallHeight * 0.5f, 0f);

        float wallX = width + wallThickness * 2f;
        wallTop.localScale = new Vector3(wallX, wallHeight, wallThickness);
        wallTop.localPosition = new Vector3(0f, wallHeight * 0.5f, halfL + wallThickness * 0.5f);
    }
}
