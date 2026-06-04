using TMPro;
using UnityEngine;

public class ProfileUI : UIBase
{
    [Header("프로필 정보 연결")]
    [SerializeField] private TextMeshProUGUI Text_PlayerName;
    [SerializeField] private TextMeshProUGUI Text_WorldName;
    [SerializeField] private TextMeshProUGUI Text_Gold;       
    [SerializeField] private TextMeshProUGUI Text_Date;

    private void OnEnable()
    {
        InitProfileData();

        if (InventoryManager.Inst != null)
        {
            InventoryManager.Inst.OnGoldChanged += UpdateGoldText;
        }
    }

    private void OnDisable()
    {
        if (InventoryManager.Inst != null)
        {
            InventoryManager.Inst.OnGoldChanged -= UpdateGoldText;
        }
    }

    private void InitProfileData()
    {
        if (GameManager.Inst != null && GameManager.Inst.PlayerData != null)
        {
            if (Text_PlayerName != null)
                Text_PlayerName.text = GameManager.Inst.PlayerData.PlayerName;

            if (Text_WorldName != null)
                Text_WorldName.text = GameManager.Inst.PlayerData.WorldName;
        }

        if (InventoryManager.Inst != null)
        {
            UpdateGoldText(InventoryManager.Inst.CurrentGold);
        }
        
        UpdateDateText();
    }

    private void UpdateGoldText(int currentGold)
    {
        if (Text_Gold != null)
        {
            Text_Gold.text = $"{currentGold:N0} G";
        }
    }

    private void UpdateDateText()
    {
        if (Text_Date != null && TimeManager.Inst != null)
        {
            string[] seasonNames = { "봄", "여름", "가을", "겨울" };

            string currentSeasonName = seasonNames[TimeManager.Inst.Season];

            Text_Date.text = $"{TimeManager.Inst.Year}년차 {currentSeasonName} {TimeManager.Inst.Day}일";
        }
    }
}
