using UnityEngine;

public sealed class PlayingState : IGameState
{
    public GameStateId Id => GameStateId.Playing;

    public void Enter(GameStateManager gsm)
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        PauseUI.Instance?.SetVisible(false);
        GameOverUI.Instance?.SetVisible(false);
    }

    public void Exit(GameStateManager gsm) { }
}