using UnityEngine;
using System.Collections;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private ArenaBuilder arena;
    [SerializeField] private CubeEntity cubePrefab;
    [SerializeField] private ScoreCubeBinder scoreBinder;

    [Header("Spawn")]
    [Min(0f)] public float clearance = 0.02f;
    [Min(0f)] public float spawnDelay = 0.3f;

    [Header("Cube size")]
    [Min(0.01f)] public float scaleMultiplier = 1f;

    private CubeEntity _current;
    public CubeEntity Current => _current;
    
    private Coroutine _spawnRoutine;

    public event System.Action<int> OnNextValueChanged;

    private int _nextValue;
    public int NextValue => _nextValue;


    private void Start()
    {
        if (GameStateManager.Instance == null || GameStateManager.Instance.IsPlaying())
        {
            RollNextValue();
            SpawnInitial();
        }           
    }

    public CubeEntity SpawnInitial()
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
            return null;

        Vector3 spawn = arena.spawnPoint.position;

        var cube = Instantiate(cubePrefab, spawn, Quaternion.identity);
        _current = cube;

        cube.SetValue(_nextValue);
        RollNextValue();

        if (scoreBinder != null)
            scoreBinder.RegisterCube(cube);

        cube.transform.localScale *= scaleMultiplier;
        cube.CacheBaseScale();

        float floorTopY = arena.GetFloorTopYWorld();
        float cubeHalfHeight = GetCubeHalfHeightWorld(cube);

        spawn.y = floorTopY + cubeHalfHeight + clearance;
        cube.transform.position = spawn;

        var rb = cube.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        return cube;
    }

    private void RollNextValue()
    {
        _nextValue = Random.value < 0.75f ? 2 : 4;
        OnNextValueChanged?.Invoke(_nextValue);
    }

    private float GetCubeHalfHeightWorld(CubeEntity cube)
    {
        var col = cube.GetComponent<BoxCollider>();
        if (col == null)
            return 0.5f * cube.transform.lossyScale.y;

        float localHalf = col.size.y * 0.5f;
        return localHalf * cube.transform.lossyScale.y;
    }

    public void SpawnNextDelayed()
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
            return;

        if (_spawnRoutine != null) return;
        _spawnRoutine = StartCoroutine(SpawnNextRoutine());
    }

    private IEnumerator SpawnNextRoutine()
    {
        yield return new WaitForSecondsRealtime(spawnDelay);

        if (GameStateManager.Instance != null && !GameStateManager.Instance.IsPlaying())
        {
            _spawnRoutine = null;
            yield break;
        }

        SpawnInitial();
        _spawnRoutine = null;
    }
    public void CancelScheduledSpawn()
    {
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    public void ClearCurrentReferenceOnly()
    {
        _current = null;
    }
}
