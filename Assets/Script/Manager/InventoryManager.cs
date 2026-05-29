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
    public int EquippedHotbarIndex { get; private set; } = 0;

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
        for (int i = 0; i < _hotbarInventory.Length; i++)
        {
            if (_hotbarInventory[i].ItemDataId == itemDataId)
            {
                _hotbarInventory[i].ItemStackCount += amount;
                OnInventoryChanged?.Invoke();
                return; 
            }
        }

        for (int i = 0; i < _mainInventory.Count; i++)
        {
            if (_mainInventory[i].ItemDataId == itemDataId)
            {
                _mainInventory[i].ItemStackCount += amount;
                OnInventoryChanged?.Invoke();
                return;
            }
        }

        for (int i = 0; i < _hotbarInventory.Length; i++)
        {
            if (string.IsNullOrEmpty(_hotbarInventory[i].ItemDataId))
            {
                _hotbarInventory[i].ItemDataId = itemDataId;
                _hotbarInventory[i].ItemStackCount = amount;
                OnInventoryChanged?.Invoke();
                return;
            }
        }

        for (int i = 0; i < _mainInventory.Count; i++)
        {
            if (string.IsNullOrEmpty(_mainInventory[i].ItemDataId))
            {
                _mainInventory[i].ItemDataId = itemDataId;
                _mainInventory[i].ItemStackCount = amount;
                OnInventoryChanged?.Invoke();
                return;
            }
        }

        Debug.LogWarning("가방이 가득 찼습니다!");
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
            if (area == SlotArea.Main) _mainInventory[clickedSlotId] = firstData;
            else _hotbarInventory[clickedSlotId] = firstData;

            if (_selectedSlotArea == SlotArea.Main) _mainInventory[_selectedSlotId] = clickedData;
            else _hotbarInventory[_selectedSlotId] = clickedData;
        }

        _selectedSlotId = -1;
        OnInventoryChanged?.Invoke();
    }
    public void ResetSelection()
    {
        _selectedSlotId = -1;
    }

    public ItemModel GetHotbarItem(int hotbarIndex)
    {
        if (hotbarIndex < 0 || hotbarIndex >= _hotbarInventory.Length)
        {
            return new ItemModel { ItemDataId = "", ItemStackCount = 0 };
        }
        return _hotbarInventory[hotbarIndex];
    }

    public void ChangeEquippedIndex(int index)
    {
        if (index <0 || index >= hotbarSize) { return; }
        EquippedHotbarIndex = index;
        OnInventoryChanged?.Invoke();
    }

    public void ConsumeHotbarItem(int hotbarIndex)
    {
        if (hotbarIndex < 0 || hotbarIndex >= _hotbarInventory.Length) { return; }
        ItemModel item = _hotbarInventory[hotbarIndex];

        if (string.IsNullOrEmpty(item.ItemDataId) || item.ItemStackCount <= 0) { return; }

        item.ItemStackCount--;

        if (item.ItemStackCount <= 0)
        {
            item.ItemDataId = "";
            item.ItemStackCount = 0;
        }

        _hotbarInventory[hotbarIndex] = item;
        OnInventoryChanged?.Invoke();
    }


    public void UseEquippedItem()
    {
        ItemModel currentItem = _hotbarInventory[EquippedHotbarIndex];

        if (string.IsNullOrEmpty(currentItem.ItemDataId) || currentItem.ItemStackCount <= 0) { return; }

        ItemUseResult result = ItemEffectManager.Inst.ApplyEffect(currentItem.ItemDataId);

        if (result == ItemUseResult.Consume) 
        {
            ConsumeHotbarItem(EquippedHotbarIndex);
        }
        else if (result == ItemUseResult.Keep)
        {
            //
        }
        else if (result == ItemUseResult.Fail)
        {
            //
        }
        
    }
}
