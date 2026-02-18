using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }

    [SerializeField] private int Score;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;
        Debug.LogWarning(amount);
        Score += amount;
    }

    public int GetScore()
    {
        return Score;
    }

    public void ResetScore()
    {
        Score = 0;
    }
}