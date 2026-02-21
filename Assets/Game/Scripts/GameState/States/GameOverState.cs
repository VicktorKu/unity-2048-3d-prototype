using UnityEngine;

public class GameOverState : IGameState
{
    public GameStateId Id => GameStateId.GameOver;

    public void Enter(GameStateManager gsm)
    {
        Time.timeScale = 0f;
        AudioListener.pause = false;

        PauseUI.Instance?.SetVisible(false);
        GameOverUI.Instance?.SetVisible(true);
    }

    public void Exit(GameStateManager gsm)
    {
        GameOverUI.Instance?.SetVisible(false);
    }
}