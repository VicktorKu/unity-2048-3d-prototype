using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public event Action<GameStateId, GameStateId> OnStateChanged;

    public GameStateId CurrentId => _current?.Id ?? default;

    private readonly Dictionary<GameStateId, IGameState> _states = new();
    private IGameState _current;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Register(new PlayingState());
        Register(new PausedState());
        Register(new GameOverState());
    }

    private void Start()
    {
        ChangeState(GameStateId.Playing);
    }

    private void Register(IGameState state) => _states[state.Id] = state;

    public void ChangeState(GameStateId next)
    {
        if (_current != null && _current.Id == next) return;
        if (!_states.TryGetValue(next, out var nextState))
        {
            Debug.LogError($"State not registered: {next}");
            return;
        }

        var prevId = _current?.Id ?? default;

        _current?.Exit(this);
        _current = nextState;
        _current.Enter(this);

        OnStateChanged?.Invoke(prevId, next);
    }

    public void TogglePause()
    {
        if (CurrentId == GameStateId.Paused) ChangeState(GameStateId.Playing);
        else if (CurrentId == GameStateId.Playing) ChangeState(GameStateId.Paused);
    }

    public bool IsPlaying() => CurrentId == GameStateId.Playing;
    public bool IsPaused() => CurrentId == GameStateId.Paused;
    public bool IsGameOver() => CurrentId == GameStateId.GameOver;
}