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
    public int EquippedHotbarIndex { get; private set; } = 0;
    public ItemModel ItemInMouse { get; private set; } = null;
    public int CurrentGold { get; private set; } = 1000;


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

    public void SwapItemWithMouse(IList<ItemModel> targetArray, int slotIndex)
    {
        ItemModel clickedItem = targetArray[slotIndex];
        ItemModel currentMouseItem = ItemInMouse;

        if (clickedItem == null) clickedItem = new ItemModel { ItemDataId = "", ItemStackCount = 0 };
        if (currentMouseItem == null) currentMouseItem = new ItemModel { ItemDataId = "", ItemStackCount = 0 };

        bool isMouseEmpty = string.IsNullOrEmpty(currentMouseItem.ItemDataId) || currentMouseItem.ItemStackCount <= 0;
        bool isClickedEmpty = string.IsNullOrEmpty(clickedItem.ItemDataId) || clickedItem.ItemStackCount <= 0;

        if (!isMouseEmpty && !isClickedEmpty && currentMouseItem.ItemDataId == clickedItem.ItemDataId)
        {
            clickedItem.ItemStackCount += currentMouseItem.ItemStackCount;
            SetItemInMouse(null); 
        }
        else
        {
            targetArray[slotIndex] = isMouseEmpty ? new ItemModel { ItemDataId = "", ItemStackCount = 0 } : currentMouseItem;
            SetItemInMouse(isClickedEmpty ? null : clickedItem);
        }

        OnInventoryChanged?.Invoke();
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
    public void SetItemInMouse(ItemModel item)
    {
        ItemInMouse = item;
        if (item != null && !string.IsNullOrEmpty(item.ItemDataId))
            Debug.Log($"마우스가 집어듬: {item.ItemDataId} ({item.ItemStackCount}개)");
        else
            Debug.Log("마우스 비어있음");
    }

    public void AddGold(int amount)
    {
        CurrentGold += amount;
        Debug.Log($"{amount} 골드 획득! (현재: {CurrentGold}G)");
        // TODO: 골드 UI 업데이트 함수 호출
    }

    public bool TryUseGold(int amount)
    {
        if (CurrentGold >= amount)
        {
            CurrentGold -= amount;
            Debug.Log($"{amount} 골드 지불 완료. (잔액: {CurrentGold}G)");
            // TODO: 골드 UI 업데이트 함수 호출
            return true;
        }
        Debug.LogWarning("골드가 부족합니다!");
        return false;
    }
    public ItemModel GetInventoryItem(int index)
    {
        if (index < 0 || index >= _mainInventory.Count)
        {
            return new ItemModel { ItemDataId = "", ItemStackCount = 0 };
        }
        return _mainInventory[index];
    }

    public void ConsumeItem(int index, int amount)
    {
        if (index < 0 || index >= _mainInventory.Count) return;

        ItemModel item = _mainInventory[index];

        if (string.IsNullOrEmpty(item.ItemDataId) || item.ItemStackCount <= 0) return;

        item.ItemStackCount -= amount;

        if (item.ItemStackCount <= 0)
        {
            item.ItemDataId = "";
            item.ItemStackCount = 0;
        }

        OnInventoryChanged?.Invoke();
    }
}
