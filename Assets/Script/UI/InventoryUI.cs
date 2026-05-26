using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : UIBase
{
    [SerializeField] private GameObject Prefab_Slot;
    [SerializeField] private Transform Transform_UISlotRoot;
    [SerializeField] private ButtonUI Button_CloseSelf;

    private int _generatedKey = 0;
    private Dictionary<int, InventorySlotUI> _itemSlotList = new();

    protected override void Awake()
    {
        base.Awake(); 

        if (Button_CloseSelf != null)
        {
            Button_CloseSelf.BindOnClickButtonEvent(OnClick_ClosePopup);
        }
    }

    private void OnEnable()
    {
        RefreshInventorySlots();
    }

    private void RefreshInventorySlots()
    {
        if (InventoryManager.Inst == null) { return; }

        var itemList = InventoryManager.Inst.GetMainInventory();
        if (itemList == null || itemList.Count == 0) { return; }

        if (_itemSlotList.Count == 0)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                CreateSlot();
            }
        }

        int index = 0;
        foreach (var slotkv in _itemSlotList)
        {
            var slotUI = slotkv.Value;
            if (index < itemList.Count)
            {
                var data = itemList[index];

                slotUI.UpdateSlot(data);
            }
            else
            {
                slotUI.UpdateSlot(null);
            }
            index++;
        }
    }

    private void CreateSlot()
    {
        var gobj = Instantiate(Prefab_Slot, Transform_UISlotRoot);
        if (gobj == null) { return; }

        gobj.transform.localScale = Vector3.one;

        var slotComponent = gobj.GetComponent<InventorySlotUI>();
        if (slotComponent == null) return;

        _generatedKey++;
        gobj.name = $"ItemSlot: {_generatedKey}";
        slotComponent.InitSlot(_generatedKey);

        _itemSlotList.Add(_generatedKey, slotComponent);
        slotComponent.BindSlotSelectEvent(OnChildSlotSelected);
    }

    private void OnChildSlotSelected(int selectedSlotInstanceId)
    {
        foreach (var slotkv in _itemSlotList)
        {
            var slot = slotkv.Value;
            bool isSlotSelected = (selectedSlotInstanceId == slot.SlotInstanceId);
            slot.ChangeSelectedState(isSlotSelected);
        }
    }

    public void OnClick_ClosePopup()
    {
        gameObject.SetActive(false);
    }
}