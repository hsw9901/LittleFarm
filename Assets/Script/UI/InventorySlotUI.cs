using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void InitSlot(int slotInstanceId)
    {
        SlotInstanceId = slotInstanceId;

        Image_Icon.gameObject.SetActive(false);
        Text_StackCount.text = "";
    }

    public void UpdateSlot(ItemModel item)
    {
        if (item == null || string.IsNullOrEmpty(item.ItemDataId) || item.ItemStackCount <= 0)
        {
            Image_Icon.gameObject.SetActive(false);
            Text_StackCount.text = "";
            return;
        }

        SetIcon(item.ItemDataId).Forget();
        Text_StackCount.text = item.ItemStackCount.ToString();
    }

    public async UniTaskVoid SetIcon(string itemDataId)
    {
        if (!GameDataManager.Inst.ItemDataList.TryGetValue(itemDataId, out ItemData data))
        {
            Debug.LogWarning($"데이터를 찾을 수 없습니다: {itemDataId}");
            return;
        }
        string key = data.IconKey;

        Sprite loadedSprite = await ResourceManager.Inst.LoadAsset<Sprite>(key);

        if (this == null || Image_Icon == null)
        {
            return; 
        }

        if (loadedSprite != null)
        {
            Image_Icon.sprite = loadedSprite;
            Image_Icon.gameObject.SetActive(true);
        }
    }

    public void OnClick_SelectItem()
    {
        //if (Image_Icon.gameObject.activeSelf == false) { return; }
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