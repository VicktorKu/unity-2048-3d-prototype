using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }

    private const string BestKey = "best_score";

    [SerializeField] private int score;
    [SerializeField] private int bestScore;
    
    public event System.Action<int, int> OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        bestScore = PlayerPrefs.GetInt(BestKey, 0);
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;

        score += amount;

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt(BestKey, bestScore);
            PlayerPrefs.Save();
        }
        
        OnScoreChanged?.Invoke(score, bestScore);
    }

    public int GetScore() => score;
    public int GetBestScore() => bestScore;

    public void ResetScore()
    {
        score = 0;
        OnScoreChanged?.Invoke(score, bestScore);
    }
}