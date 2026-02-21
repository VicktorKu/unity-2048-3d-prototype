public interface IGameState
{
    GameStateId Id { get; }
    void Enter(GameStateManager gsm);
    void Exit(GameStateManager gsm);
}