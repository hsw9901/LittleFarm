using UnityEngine;
using System;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Inst {  get; private set; }

    public enum SlotArea { Main, Hotbar }

    [Header("인벤토리 설정")]
    [SerializeField] private int mainInventorySize = 36;
    [SerializeField] private int hotbarSize = 10;

    private List<ItemModel> _mainInventory = new List<ItemModel>();
    private ItemModel[] _hotbarInventory;

    public event Action OnInventoryChanged;
    public int _selectedSlotId = -1;
    public int SelectedSlotId => _selectedSlotId;

    public SlotArea _selectedSlotArea = SlotArea.Main;
    public SlotArea SelectedSlotArea => _selectedSlotArea;

    private void Awake()
    {
        Inst = this;
        InitInventory();
    }

    private void InitInventory()
    {
        _mainInventory.Clear();
        for (int i = 0; i < mainInventorySize; i++)
        {
            _mainInventory.Add(new ItemModel { ItemDataId = "", ItemStackCount = 0 });
        }
        _hotbarInventory = new ItemModel[hotbarSize];
        for (int i = 0; i < hotbarSize; i++)
        {
            _hotbarInventory[i] = new ItemModel { ItemDataId = "", ItemStackCount = 0 };
        }
    }

    public List<ItemModel> GetMainInventory() => _mainInventory;
    public ItemModel[] GetHotbarInventory() => _hotbarInventory;

    public void ApplySaveData(List<ItemModel> main, ItemModel[] hotbar)
    {
        if (main != null) { _mainInventory = main; }
        if (hotbar != null) { _hotbarInventory = hotbar; }  

        OnInventoryChanged?.Invoke();
    }

    public void AddItem(string itemDataId, int amount)
    {
        bool isItemFound = false;

        foreach (var item in _mainInventory)
        {
            if (item.ItemDataId == itemDataId)
            {
                item.ItemStackCount += amount;
                isItemFound = true;
                break;
            }
        }

        if (!isItemFound)
        {
            foreach (var item in _mainInventory)
            {
                if (string.IsNullOrEmpty(item.ItemDataId))
                {
                    item.ItemDataId = itemDataId;
                    item.ItemStackCount = amount;
                    break;
                }
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public void ClickHandSlot(int clickedSlotId, SlotArea area)
    {
        if (area == SlotArea.Main && (clickedSlotId < 0 || clickedSlotId >= _mainInventory.Count)) return;
        if (area == SlotArea.Hotbar && (clickedSlotId < 0 || clickedSlotId >= _hotbarInventory.Length)) return;

        ItemModel clickedData = (area == SlotArea.Main) ? _mainInventory[clickedSlotId] : _hotbarInventory[clickedSlotId];

        if (_selectedSlotId == -1 && string.IsNullOrEmpty(clickedData.ItemDataId)) return;

        if (_selectedSlotId == -1)
        {
            _selectedSlotId = clickedSlotId;
            _selectedSlotArea = area; 
            OnInventoryChanged?.Invoke();
            return;
        }

        if (_selectedSlotId == clickedSlotId && _selectedSlotArea == area)
        {
            _selectedSlotId = -1;
            OnInventoryChanged?.Invoke();
            return;
        }

        ItemModel firstData = (_selectedSlotArea == SlotArea.Main) ? _mainInventory[_selectedSlotId] : _hotbarInventory[_selectedSlotId];

        if (firstData.ItemDataId == clickedData.ItemDataId && !string.IsNullOrEmpty(firstData.ItemDataId))
        {
            clickedData.ItemStackCount += firstData.ItemStackCount;
            firstData.ItemDataId = "";
            firstData.ItemStackCount = 0;
        }
        else
        {
            string tempId = clickedData.ItemDataId;
            int tempCount = clickedData.ItemStackCount;

            clickedData.ItemDataId = firstData.ItemDataId;
            clickedData.ItemStackCount = firstData.ItemStackCount;

            firstData.ItemDataId = tempId;
            firstData.ItemStackCount = tempCount;
        }

        if (area == SlotArea.Main) _mainInventory[clickedSlotId] = clickedData;
        else _hotbarInventory[clickedSlotId] = clickedData;

        if (_selectedSlotArea == SlotArea.Main) _mainInventory[_selectedSlotId] = firstData;
        else _hotbarInventory[_selectedSlotId] = firstData;

        _selectedSlotId = -1;
        OnInventoryChanged?.Invoke();
    }
    public void ResetSelection()
    {
        _selectedSlotId = -1;
    }
}
