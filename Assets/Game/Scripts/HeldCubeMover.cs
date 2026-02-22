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
    [Range(0f, 1f)] public float bottomScreenZone = 0.35f;
    
    [Header("Power control")]
    [Min(0f)] public float maxPullBackDistance = 0.6f;
    [Range(0f, 1f)] public float minPowerPercent = 0.5f;
    [Min(1f)] public float pullBackPixelsForMax = 220f;

    [SerializeField] private LaunchPowerUI powerUI;
    
    private bool _holding;
    private float _grabOffsetXWorld;
    private float _grabStartScreenY;
    private Vector3 _holdStartWorldPos;
    private float _currentPower01 = 0f;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
        {
            if (_holding) CancelHold();
            return;
        }

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
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
            return;

        _holding = true;

        _grabStartScreenY = screenPos.y;
        _holdStartWorldPos = cube.transform.position;
        _currentPower01 = 0f;
        powerUI?.SetPull01(_currentPower01);

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
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
            return;

        float pointerXWorld = ScreenXToWorldX(screenPos, cube.transform.position);
        float desiredX = pointerXWorld + _grabOffsetXWorld;

        float halfW = arena.width * 0.5f;
        float cubeHalfWidth = GetCubeHalfWidthWorld(cube);

        float centerX = arena.transform.position.x;
        float minX = centerX - halfW + cubeHalfWidth + sideClearance;
        float maxX = centerX + halfW - cubeHalfWidth - sideClearance;

        float dy = (_grabStartScreenY - screenPos.y);
        float pull01 = Mathf.Clamp01(dy / pullBackPixelsForMax);

        float pullWorld = pull01 * maxPullBackDistance;

        var pos = _holdStartWorldPos;
        pos.x = Mathf.Clamp(desiredX, minX, maxX);

        pos += -arena.transform.forward * pullWorld;

        cube.transform.position = pos;

        _currentPower01 = pull01;
        powerUI?.SetPull01(_currentPower01);
    }

    private float ScreenXToWorldX(Vector2 screenPos, Vector3 cubeWorldPos)
    {
        float z = cam.WorldToScreenPoint(cubeWorldPos).z;
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, z));
        return world.x;
    }
    private void CancelHold()
    {
        _holding = false;
        _currentPower01 = 0f;
        powerUI?.HideFill();
    }

    private void ReleaseAndLaunch(CubeEntity cube)
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
            return;

        _holding = false;

        cube.MarkLaunched();

        float powerMul = Mathf.Lerp(minPowerPercent, 1f, _currentPower01);

        var rb = cube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(arena.transform.forward * (launchImpulse * powerMul), ForceMode.Impulse);
        }

        spawner.ClearCurrentReferenceOnly();
        spawner.SpawnNextDelayed();

        powerUI?.HideFill();

        AudioManager.Instance?.PlayLaunch();

        FindObjectOfType<LaunchHint>()?.OnFirstLaunch();
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
