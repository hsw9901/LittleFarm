using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    private void Awake()
    {
        Inst = this;
    }
    public void RequestStartGame()
    {
        GameStateManager.Inst.ChangeState(GameState.Playing);
    }

    public void RequestPauseToggle()
    {
        if (GameStateManager.Inst.IsPlaying)
        { 
            GameStateManager.Inst.ChangeState(GameState.Paused);
        }

        else if (GameStateManager.Inst.IsPaused)
        {
            GameStateManager.Inst.ChangeState(GameState.Playing);
        }

        
    }
    public void RequestMainMenu() 
    {
        GameStateManager.Inst.ChangeState(GameState.MainMenu);
    }
}
