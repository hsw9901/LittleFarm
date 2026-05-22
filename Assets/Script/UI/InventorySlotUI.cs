using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text_StackCount;
    [SerializeField] private ButtonUI Button_Slot;
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_Selected;

    private event Action<int> _onSelectEvent;

    public int SlotInstanceId { get; private set; }

    private void OnEnable()
    {
        Image_Selected.gameObject.SetActive(false);

        Button_Slot.BindOnClickButtonEvent(OnClick_SelectItem);
    }

    private void OnDisable()
    {
        _onSelectEvent = null;
    }

    public void InitSlot(int slotInstanceId, string itemDataId, int itemStackCount)
    {
        SlotInstanceId = slotInstanceId;

        if (string.IsNullOrEmpty(itemDataId) || itemStackCount <= 0)
        {
            Image_Icon.gameObject.SetActive(false);
            Text_StackCount.text = "";
            return;
        }

        Image_Icon.gameObject.SetActive(true);
        SetIcon(itemDataId, itemStackCount);
    }

    public void SetIcon(string itemDataId, int itemCount)
    {

        Sprite loadedSprite = Resources.Load<Sprite>($"Icons/{itemDataId}");

        if (loadedSprite != null)
        {
            Image_Icon.sprite = loadedSprite;
        }
        else
        {
            Debug.LogWarning($"[InventorySlotUI] 아이콘 이미지를 찾을 수 없습니다. 경로: Resources/Icons/{itemDataId}");
        }

        Text_StackCount.text = $"{itemCount}";
    }

    public void OnClick_SelectItem()
    {
        if (Image_Icon.gameObject.activeSelf == false) { return; }
        _onSelectEvent?.Invoke(SlotInstanceId);
    }

    public void BindSlotSelectEvent(Action<int> onSelectEvent)
    {
        _onSelectEvent = onSelectEvent;
    }

    public void ChangeSelectedState(bool isSelected)
    {
        Image_Selected.gameObject.SetActive(isSelected);
    }
}