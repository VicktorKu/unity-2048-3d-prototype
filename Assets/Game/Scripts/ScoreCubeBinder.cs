using UnityEngine;

public class ScoreCubeBinder : MonoBehaviour
{
    [SerializeField] private ScoreSystem scoreSystem;

    private void Awake()
    {
        if (scoreSystem == null)
            scoreSystem = ScoreSystem.Instance;
    }

    private void Start()
    {
        var cubes = FindObjectsOfType<CubeEntity>();
        for (int i = 0; i < cubes.Length; i++)
            RegisterCube(cubes[i]);
    }

    public void RegisterCube(CubeEntity cube)
    {
        if (cube == null || scoreSystem == null) return;

        cube.OnValueChanged -= OnCubeMergedBaseValue;
        cube.OnValueChanged += OnCubeMergedBaseValue;
    }

    private void OnCubeMergedBaseValue(int baseValue)
    {
        scoreSystem.Add(baseValue/2);
    }
}