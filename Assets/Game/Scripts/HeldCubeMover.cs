using UnityEngine;

public class HeldCubeMover : MonoBehaviour
{
    [SerializeField] private ArenaBuilder arena;
    [SerializeField] private CubeSpawner spawner;

    [Header("Move")]
    [Min(0.00001f)] public float pixelsToWorld = 0.01f; 
    [Min(0f)] public float sideClearance = 0.05f; 

    private bool _holding;
    private float _startPointerX;
    private float _startCubeX;

    void Update()
    {
        var cube = spawner.Current;
        if (cube == null) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
            BeginHold(cube.transform.position.x, Input.mousePosition.x);

        if (Input.GetMouseButton(0) && _holding)
            HoldMove(cube);

        if (Input.GetMouseButtonUp(0))
            _holding = false;
#else
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
                BeginHold(cube.transform.position.x, t.position.x);

            if ((t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) && _holding)
                HoldMove(cube);

            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                _holding = false;
        }
#endif
    }

    private void BeginHold(float cubeX, float pointerX)
    {
        _holding = true;
        _startCubeX = cubeX;
        _startPointerX = pointerX;
    }

    private void HoldMove(CubeEntity cube)
    {
        float deltaPixels =
#if UNITY_EDITOR || UNITY_STANDALONE
            Input.mousePosition.x - _startPointerX;
#else
            Input.GetTouch(0).position.x - _startPointerX;
#endif

        float desiredX = _startCubeX + deltaPixels * pixelsToWorld;

        float halfW = arena.width * 0.5f;

        float cubeHalfWidth = GetCubeHalfWidthWorld(cube);

        float minX = arena.transform.position.x - halfW + cubeHalfWidth + sideClearance;
        float maxX = arena.transform.position.x + halfW - cubeHalfWidth - sideClearance;

        var pos = cube.transform.position;
        pos.x = Mathf.Clamp(desiredX, minX, maxX);
        cube.transform.position = pos;
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
