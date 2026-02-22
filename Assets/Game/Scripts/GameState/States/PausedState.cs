using UnityEngine;

public class PausedState : IGameState
{
    public GameStateId Id => GameStateId.Paused;

    public void Enter(GameStateManager gsm)
    {
        Time.timeScale = 0f;
        //AudioListener.pause = true;

        PauseUI.Instance?.SetVisible(true);
    }

    public void Exit(GameStateManager gsm)
    {
        PauseUI.Instance?.SetVisible(false);
    }
}