using UnityEngine;
using TMPro;

public class NewGamePopupUI : UIBase
{
    [Header("입력칸 연결")]
    [SerializeField] private TMP_InputField Input_CharacterName;
    [SerializeField] private TMP_InputField Input_WorldName;

    [Header("버튼 연결")] 
    [SerializeField] private ButtonUI Btn_Confirm;
    [SerializeField] private ButtonUI Btn_Cancel;

    private void Start()
    {
        if (Btn_Confirm != null) Btn_Confirm.BindOnClickButtonEvent(OnClick_ConfirmButton);
        if (Btn_Cancel != null) Btn_Cancel.BindOnClickButtonEvent(OnClick_CancelButton);
    }

    private void OnClick_ConfirmButton()
    {
        string charName = Input_CharacterName.text;
        string worldName = Input_WorldName.text;

        if (string.IsNullOrEmpty(charName) || string.IsNullOrEmpty(worldName))
        {
            UIManager.Inst.OpenSimplePopup("<color=red>이름을 모두 입력해주세요!</color>");
            return;
        }

        UIManager.Inst.CloseContentUI(UIType.NewGamePopup);

        GameManager.Inst.StartNewGame(charName, worldName);
    }

    public void OnClick_CancelButton()
    {
        UIManager.Inst.CloseContentUI(UIType.NewGamePopup);
    }
}