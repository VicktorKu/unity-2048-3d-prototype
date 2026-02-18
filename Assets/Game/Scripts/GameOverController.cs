using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public static GameOverController Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestText;

    [Header("Disable On Game Over")]
    [SerializeField] private MonoBehaviour[] disableBehaviours;

    private const string BestKey = "best_score";
    private bool _isGameOver;

    public bool IsGameOver => _isGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (_isGameOver) return;
        _isGameOver = true;

        int score = ScoreSystem.Instance != null ? ScoreSystem.Instance.GetScore() : 0;

        int best = PlayerPrefs.GetInt(BestKey, 0);
        if (score > best)
        {
            best = score;
            PlayerPrefs.SetInt(BestKey, best);
            PlayerPrefs.Save();
        }

        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (bestText != null) bestText.text = $"Best: {best}";

        DisableGameSystems();

        if (panel != null)
            panel.SetActive(true);

        AudioManager.Instance?.PlayGameOver();
    }

    private void DisableGameSystems()
    {
        if (disableBehaviours == null) return;

        for (int i = 0; i < disableBehaviours.Length; i++)
        {
            if (disableBehaviours[i] != null)
                disableBehaviours[i].enabled = false;
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}