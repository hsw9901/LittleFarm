using UnityEngine;
using System;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Inst {  get; private set; }

    [Header("인벤토리 설정")]
    [SerializeField] private int mainInventorySize = 36;
    [SerializeField] private int hotbarSize = 6;

    private List<ItemModel> _mainInventory = new List<ItemModel>();
    private ItemModel[] _hotbarInventory;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        Inst = this;
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
}
