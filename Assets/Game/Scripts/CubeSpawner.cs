using UnityEngine;
using System.Collections;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private ArenaBuilder arena;
    [SerializeField] private CubeEntity cubePrefab;

    [Header("Spawn")]
    [Min(0f)] public float clearance = 0.02f;
    [Min(0f)] public float spawnDelay = 0.3f;

    [Header("Cube size")]
    [Min(0.01f)] public float scaleMultiplier = 1f;

    private CubeEntity _current;
    public CubeEntity Current => _current;
    
    private Coroutine _spawnRoutine;


    private void Start()
    {
        SpawnInitial();
    }

    public CubeEntity SpawnInitial()
    {
        Vector3 spawn = arena.spawnPoint.position;

        var cube = Instantiate(cubePrefab, spawn, Quaternion.identity);
        _current = cube;

        cube.transform.localScale *= scaleMultiplier;

        float floorTopY = arena.GetFloorTopYWorld();
        float cubeHalfHeight = GetCubeHalfHeightWorld(cube);

        spawn.y = floorTopY + cubeHalfHeight + clearance;
        cube.transform.position = spawn;

        var rb = cube.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        int v = Random.value < 0.75f ? 2 : 4;
        cube.SetValue(v);

        return cube;
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
        if (_spawnRoutine != null) return;
        _spawnRoutine = StartCoroutine(SpawnNextRoutine());
    }

    private IEnumerator SpawnNextRoutine()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnInitial();
        _spawnRoutine = null;
    }

    public void ClearCurrentReferenceOnly()
    {
        _current = null;
    }
}
