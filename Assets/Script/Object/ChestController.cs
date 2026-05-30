using UnityEngine;
using System.Collections.Generic;

public class ChestController : UIBase
{
    [SerializeField] private GameObject Prefab_Slot;      
    [SerializeField] private Transform Transform_SlotRoot;

    private Chest _currentChest;
    private List<InventorySlotUI> _chestSlotList = new List<InventorySlotUI>();

    private void OnEnable()
    {
        if (InventoryManager.Inst != null)
        {
            InventoryManager.Inst.OnInventoryChanged += RefreshAllSlot;
        }
    }

    private void OnDisable()
    {
        if (InventoryManager.Inst != null)
        {
            InventoryManager.Inst.OnInventoryChanged -= RefreshAllSlot;
        }
    }

    public void OpenChestUI(Chest chestData)
    {
        _currentChest = chestData;
        if (_chestSlotList.Count == 0)
        {
            InitChestSlots(chestData.ChestSize);
        }

        RefreshAllSlot();

    }

    private void InitChestSlots(int size)
    {
        if (Prefab_Slot == null || Transform_SlotRoot == null) return;

        for (int i = 0; i < size; i++)
        {
            var gobj = Instantiate(Prefab_Slot, Transform_SlotRoot);
            gobj.transform.localScale = Vector3.one;

            int slotIndex = i; 
            gobj.name = $"ChestSlot: {slotIndex}";

            var slotUI = gobj.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.InitSlot(slotIndex, InventoryManager.SlotArea.Main);
                slotUI.OnClicked = (id) => OnSlotClicked(true, slotIndex);

                _chestSlotList.Add(slotUI);
            }
        }
    }

    public void OnClick_ClosePopup()
    {
        if (UIManager.Inst != null)
        {
            UIManager.Inst.ClosePopupUI(UIType.ChestUI);
        }
    }

    public void CloseChestUI()
    {
        _currentChest = null;
        gameObject.SetActive(false);
    }

    public void OnSlotClicked(bool isChestSlot, int slotIndex)
    {
        if (_currentChest == null || InventoryManager.Inst == null) return;

        IList<ItemModel> targetArray = isChestSlot
            ? (IList<ItemModel>)_currentChest.ChestInventory
            : (IList<ItemModel>)InventoryManager.Inst.GetMainInventory();

        InventoryManager.Inst.SwapItemWithMouse(targetArray, slotIndex);
    }

    private void RefreshAllSlot()
    {
        if (_currentChest == null) return;

        for (int i = 0; i < _chestSlotList.Count; i++)
        {
            var itemData = _currentChest.ChestInventory[i];

            _chestSlotList[i].ChangeSelectedState(false);
            _chestSlotList[i].UpdateSlot(itemData);
        }
    }
}
