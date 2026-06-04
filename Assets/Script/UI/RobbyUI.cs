using UnityEngine;

public class RobbyUI : UIBase
{
    [SerializeField] private ButtonUI Button_GameStart;
    [SerializeField] private ButtonUI Button_GameLoad;
    [SerializeField] private ButtonUI Button_GameQuit;

    private void OnEnable()
    {
        Button_GameStart.BindOnClickButtonEvent(OnClick_GameStart);
        Button_GameLoad.BindOnClickButtonEvent(OnClick_GameLoad);
        Button_GameQuit.BindOnClickButtonEvent(OnClick_GameQuit);

    }

    public void OnClick_GameStart()
    {
        UIManager.Inst.CloseContentUI(UIType.RobbyUI);
    }
    public void OnClick_GameLoad()
    {
        GameManager.Inst.LoadGame();
    }

    public void OnClick_GameQuit()
    {
        GameManager.Inst.SaveAndEndGame();
    }
}
