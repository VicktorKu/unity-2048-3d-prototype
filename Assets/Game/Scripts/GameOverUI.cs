using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestText;

    [SerializeField] private Transform statsRoot;
    [SerializeField] private GameOverCubeStatEntry entryPrefab;
    [SerializeField] private CubeColorConfig visualConfig;

    private void Awake()
    {
        Instance = this;
        SetVisible(false);
    }

    public void SetVisible(bool visible)
    {
        if (panel != null) panel.SetActive(visible);
        else gameObject.SetActive(visible);
    }

    public void SetData(int score, int best)
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (bestText != null) bestText.text = $"Best: {best}";
    }

    public void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowGameOver(int score, int best)
    {
        SetData(score, best);
        BuildRoundStats();
        SetVisible(true);
    }

    private void BuildRoundStats()
    {     
        if (!statsRoot || !entryPrefab) return;

        for (int i = statsRoot.childCount - 1; i >= 0; i--)
            Destroy(statsRoot.GetChild(i).gameObject);

        var dict = CubeStatsManager.Instance != null
            ? CubeStatsManager.Instance.RoundMerged
            : null;

        if (dict == null || dict.Count == 0) return;

        foreach (var kv in dict.OrderByDescending(x => x.Key))
        {
            var value = kv.Key;
            var count = kv.Value;

            var e = Instantiate(entryPrefab, statsRoot);

            var c = Color.white;
            if (visualConfig) visualConfig.TryGetColor(value, out c);

            e.Setup(value, count, c);
        }
    }
}