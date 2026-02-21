using UnityEngine;

public class BestScoreRepository
{
    private const string BestKey = "best_score";

    public int GetBest() => PlayerPrefs.GetInt(BestKey, 0);

    public int SaveIfBetter(int score)
    {
        int best = GetBest();
        if (score <= best) return best;

        PlayerPrefs.SetInt(BestKey, score);
        PlayerPrefs.Save();
        return score;
    }
}