using System;
using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : UIBase
{
    [Header("프리팹 및 생성 영역")]
    [SerializeField] private GameObject Prefab_Slot; 
    [SerializeField] private Transform Transform_HotbarRoot; 

    [Header("툴바 설정")]
    [SerializeField] private int HotbarSlotCount = 10;

    private List<InventorySlotUI> _hotbarSlotList = new List<InventorySlotUI>();

    protected override void Awake()
    {
        base.Awake();
        InitHotbarSlots();
    }

    private void Start()
    {
        if (InventoryManager.Inst != null)
        {
            InventoryManager.Inst.OnInventoryChanged -= RefreshHotbarSlots;
            InventoryManager.Inst.OnInventoryChanged += RefreshHotbarSlots;
            RefreshHotbarSlots();
        }

        SelectSlot(0);
    }

    private void InitHotbarSlots()
    {
        if (Prefab_Slot == null || Transform_HotbarRoot == null) { return; }

        for (int i = 0; i < HotbarSlotCount; i++)
        {
            var gobj = Instantiate(Prefab_Slot, Transform_HotbarRoot);
            gobj.transform.localScale = Vector3.one; 
            gobj.name = $"HotbarSlot: {i}";

            var slotComponent = gobj.GetComponent<InventorySlotUI>();
            if (slotComponent != null)
            {
                _hotbarSlotList.Add(slotComponent);

                int index = i;
                slotComponent.InitSlot(i, InventoryManager.SlotArea.Hotbar);

                slotComponent.OnClicked = (id) => OnClick_HotbarSlot(index);
            }
        }
    }

    public void RefreshHotbarSlots()
    {
        if (InventoryManager.Inst == null) { return; }

        var hotbarDataArray = InventoryManager.Inst.GetHotbarInventory();
        if (hotbarDataArray == null) { return; }

        int equippedIndex = InventoryManager.Inst.EquippedHotbarIndex;

        for (int i = 0; i < _hotbarSlotList.Count; i++)
        {
            var data = hotbarDataArray[i];

            _hotbarSlotList[i].UpdateSlot(data);

            bool isEquipped = (i == equippedIndex);
            _hotbarSlotList[i].ChangeSelectedState(isEquipped);
        }
    }

   
    private void OnClick_HotbarSlot(int index)
    {
        if (InventoryManager.Inst == null) return;

        bool isInventoryOpen = false;
        if (UIManager.Inst != null)
        {
            isInventoryOpen = UIManager.Inst.IsOpened(UIType.Inventory);
        }

        if (isInventoryOpen)
        {
            IList<ItemModel> hotbarArray = (IList<ItemModel>)InventoryManager.Inst.GetHotbarInventory();
            InventoryManager.Inst.SwapItemWithMouse(hotbarArray, index);
        }
        else
        {
            SelectSlot(index);
        }
    }

    private void SelectSlot(int index)
    {
        if (index < 0 || index >= _hotbarSlotList.Count) return;


        if (InventoryManager.Inst != null)
        {
            InventoryManager.Inst.ChangeEquippedIndex(index);
        }
    }
}