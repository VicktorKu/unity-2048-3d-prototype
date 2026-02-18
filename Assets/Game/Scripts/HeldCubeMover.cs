using UnityEngine;

public class HeldCubeMover : MonoBehaviour
{
    [SerializeField] private ArenaBuilder arena;
    [SerializeField] private CubeSpawner spawner;
    [SerializeField] private Camera cam;

    [Header("Move")]
    [Min(0f)] public float sideClearance = 0.05f;

    [Header("Launch")]
    [Min(0f)] public float launchImpulse = 12f;

    [Header("Hold area")]
    [Tooltip("Дозволяти керування лише коли палець внизу екрана (0..1)")]
    [Range(0f, 1f)] public float bottomScreenZone = 0.35f;

    private bool _holding;
    private float _grabOffsetXWorld;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        var cube = spawner.Current;
        if (cube == null || cam == null) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerInHoldZone(Input.mousePosition))
                BeginHold(cube, Input.mousePosition);
        }

        if (Input.GetMouseButton(0) && _holding)
            HoldMove(cube, Input.mousePosition);

        if (Input.GetMouseButtonUp(0) && _holding)
            ReleaseAndLaunch(cube);
#else
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                if (IsPointerInHoldZone(t.position))
                    BeginHold(cube, t.position);
            }

            if ((t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) && _holding)
                HoldMove(cube, t.position);

            if ((t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) && _holding)
                ReleaseAndLaunch(cube);
        }
#endif
    }

    private bool IsPointerInHoldZone(Vector2 screenPos)
    {
        return screenPos.y <= Screen.height * bottomScreenZone;
    }

    private void BeginHold(CubeEntity cube, Vector2 screenPos)
    {
        _holding = true;

        var rb = cube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.isKinematic = true;
        }

        float pointerXWorld = ScreenXToWorldX(screenPos, cube.transform.position);
        _grabOffsetXWorld = cube.transform.position.x - pointerXWorld;
    }

    private void HoldMove(CubeEntity cube, Vector2 screenPos)
    {
        float pointerXWorld = ScreenXToWorldX(screenPos, cube.transform.position);
        float desiredX = pointerXWorld + _grabOffsetXWorld;

        float halfW = arena.width * 0.5f;
        float cubeHalfWidth = GetCubeHalfWidthWorld(cube);

        float centerX = arena.transform.position.x;
        float minX = centerX - halfW + cubeHalfWidth + sideClearance;
        float maxX = centerX + halfW - cubeHalfWidth - sideClearance;

        var pos = cube.transform.position;
        pos.x = Mathf.Clamp(desiredX, minX, maxX);
        cube.transform.position = pos;
    }

    private float ScreenXToWorldX(Vector2 screenPos, Vector3 cubeWorldPos)
    {
        float z = cam.WorldToScreenPoint(cubeWorldPos).z;
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, z));
        return world.x;
    }

    private void ReleaseAndLaunch(CubeEntity cube)
    {
        _holding = false;

        var rb = cube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(arena.transform.forward * launchImpulse, ForceMode.Impulse);
        }

        spawner.ClearCurrentReferenceOnly();
        spawner.SpawnNextDelayed();

        AudioManager.Instance?.PlayLaunch();
    }


    private float GetCubeHalfWidthWorld(CubeEntity cube)
    {
        var col = cube.GetComponent<BoxCollider>();
        if (col == null)
            return 0.5f * cube.transform.lossyScale.x;

        float localHalf = col.size.x * 0.5f;
        return localHalf * cube.transform.lossyScale.x;
    }
}
