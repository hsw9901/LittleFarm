using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : UIBase
{
    [SerializeField] private GameObject Prefab_Slot;
    [SerializeField] private Transform Transform_UISlotRoot;
    [SerializeField] private ButtonUI Button_CloseSelf;

    private List<InventorySlotUI> _itemSlotList = new List<InventorySlotUI>();

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
        if (InventoryManager.Inst != null)
        {
            InventoryManager.Inst.OnInventoryChanged += RefreshInventorySlots;
            InventoryManager.Inst.ResetSelection(); 
        }
        RefreshInventorySlots();
    }

    private void OnDisable()
    {
        if (InventoryManager.Inst != null)
        {
            InventoryManager.Inst.OnInventoryChanged -= RefreshInventorySlots;
        }
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

        int currentSelectedId = InventoryManager.Inst.SelectedSlotId;
        InventoryManager.SlotArea currentSelectedArea = InventoryManager.Inst.SelectedSlotArea;

        for (int i = 0; i < _itemSlotList.Count; i++)
        {
            var slotUI = _itemSlotList[i];
            slotUI.InitSlot(i, InventoryManager.SlotArea.Main);

            bool amISelected = (i == currentSelectedId && currentSelectedArea == InventoryManager.SlotArea.Main);
            slotUI.ChangeSelectedState(amISelected);

            if (i < itemList.Count)
            {
                slotUI.UpdateSlot(itemList[i]);
            }
            else
            {
                slotUI.UpdateSlot(null);
            }
        }
    }

    private void CreateSlot()
    {
        var gobj = Instantiate(Prefab_Slot, Transform_UISlotRoot);
        if (gobj == null) { return; }

        gobj.transform.localScale = Vector3.one;

        var slotComponent = gobj.GetComponent<InventorySlotUI>();
        if (slotComponent == null) return;

        gobj.name = $"ItemSlot: {_itemSlotList.Count}";

        _itemSlotList.Add(slotComponent);
    }

    public void OnClick_ClosePopup()
    {
        gameObject.SetActive(false);
    }
}