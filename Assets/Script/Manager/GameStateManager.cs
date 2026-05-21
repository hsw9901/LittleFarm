using System;
using UnityEngine;



public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Inst { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    public event Action<GameState, GameState> OnStateChanged;

    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsPaused => CurrentState == GameState.Paused;

    private void Awake()
    {
        Inst = this;
    }

    public void ChangeState(GameState nextstate)
    {
        if (CurrentState == nextstate) return;

        var prevstate = CurrentState;
        CurrentState = nextstate;

        ExitState(prevstate);
        EnterState(nextstate);

        OnStateChanged?.Invoke(prevstate, nextstate);
        Debug.Log($"[GameManager] {prevstate} → {nextstate}");
    }

    private void ExitState(GameState state)
    {
        if (state == GameState.Paused)
        { Time.timeScale = 1f; }
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
        }
    }
}