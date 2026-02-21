using UnityEngine;

public class GameOverService : MonoBehaviour
{
    public static GameOverService Instance { get; private set; }

    [SerializeField] private MonoBehaviour[] disableBehaviours;

    private bool _isGameOver;
    private BestScoreRepository _bestRepo;

    public bool IsGameOver => _isGameOver;

    private void Awake()
    {
        Instance = this;
        _bestRepo = new BestScoreRepository();
    }

    public void TriggerGameOver()
    {
        if (_isGameOver) return;
        _isGameOver = true;

        int score = ScoreSystem.Instance != null ? ScoreSystem.Instance.GetScore() : 0;
        int best = _bestRepo.SaveIfBetter(score);

        DisableGameSystems();

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.ChangeState(GameStateId.GameOver);

        GameOverUI.Instance?.SetData(score, best);

        AudioManager.Instance?.PlayGameOver();
    }

    private void DisableGameSystems()
    {
        if (disableBehaviours == null) return;

        for (int i = 0; i < disableBehaviours.Length; i++)
            if (disableBehaviours[i] != null)
                disableBehaviours[i].enabled = false;
    }
}