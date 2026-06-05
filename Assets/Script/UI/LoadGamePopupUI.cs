using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LoadGamePopupUI : UIBase
{
    [Header("슬롯 생성 설정")]
    [SerializeField] private GameObject SaveSlotPrefab;   
    [SerializeField] private Transform SlotContentParent; 
    [SerializeField] private int maxSlots = 5;

    [Header("닫기 버튼")]
    [SerializeField] private ButtonUI Btn_Close;

    private List<ButtonUI> _spawnedSlots = new List<ButtonUI>();


    private void Start()
    {
        if (Btn_Close != null) Btn_Close.BindOnClickButtonEvent(OnClick_CloseButton);

        CreateSlots();
        RefreshSlots();
    }

    private void OnEnable()
    {
        RefreshSlots();
    }

    private void CreateSlots()
    {
        foreach (Transform child in SlotContentParent)
        {
            Destroy(child.gameObject);
        }
        _spawnedSlots.Clear();

        for (int i = 1; i <= maxSlots; i++)
        {
            GameObject slotObj = Instantiate(SaveSlotPrefab, SlotContentParent);
            ButtonUI btnUI = slotObj.GetComponent<ButtonUI>();

            if (btnUI != null)
            {
                int slotIndex = i;

                btnUI.BindOnClickButtonEvent(() => OnClick_LoadSlot(slotIndex));

                _spawnedSlots.Add(btnUI);
            }
        }
    }

    private void RefreshSlots()
    {
        Debug.Log($"[LoadGamePopup] 현재 리스트에 담긴 슬롯 개수: {_spawnedSlots.Count}개");
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            int slotIndex = i + 1; 
            UpdateSlotUI(_spawnedSlots[i], slotIndex);
        }
    }

    private void UpdateSlotUI(ButtonUI btn, int slotIndex)
    {
        if (btn == null) return;

        TextMeshProUGUI textUI = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (textUI == null) { return; }
        

        if (SaveManager.Inst.HasSaveData(slotIndex))
        {
            SaveModel preview = SaveManager.Inst.GetSavePreview(slotIndex);
            if (preview != null && preview.PlayerData != null)
            {
                string pName = string.IsNullOrEmpty(preview.PlayerData.PlayerName) ? "빈 슬롯" : preview.PlayerData.PlayerName;
                string wName = string.IsNullOrEmpty(preview.PlayerData.WorldName) ? "빈 슬롯" : preview.PlayerData.WorldName;

                textUI.text = $"[슬롯 {slotIndex}] {wName}\n<size=80%>{pName} - {preview.PlayerData.Gold}G</size>";
            }
        }
        else
        {
            textUI.text = $"<color=#888888>[슬롯 {slotIndex}] 비어있음</color>";
        }
    }
    private void OnClick_LoadSlot(int slotIndex)
    {
        if (!SaveManager.Inst.HasSaveData(slotIndex))
        {
            UIManager.Inst.OpenSimplePopup("<color=red>저장된 데이터가 없습니다!</color>");
            return;
        }

        UIManager.Inst.CloseContentUI(UIType.LoadGamePopup);

        GameManager.Inst.StartLoadedGame(slotIndex);
    }

    private void OnClick_CloseButton()
    {
        UIManager.Inst.CloseContentUI(UIType.LoadGamePopup);
    }
}