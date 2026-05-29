using UnityEngine;
using System.Collections.Generic;

public class ChestController : UIBase
{
    [SerializeField] private GameObject Prefab_Slot;      
    [SerializeField] private Transform Transform_SlotRoot;

    private Chest _currentChest;
    private ItemModel _itemInMouse = null;

    private List<InventorySlotUI> _chestSlotList = new List<InventorySlotUI>();

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
            gobj.name = $"ChestSlot: {i}";

            var slotUI = gobj.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                int index = i;

                // 임시로 Main으로 넘기되, 상호작용은 아래 OnClicked에서 덮어씌웁니다.
                slotUI.InitSlot(index, InventoryManager.SlotArea.Main);

                // 클릭했을 때 "상자 슬롯이야(true)!" 하고 번호를 넘겨줍니다.
                slotUI.OnClicked = (id) => OnSlotClicked(true, index);

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
        if (_currentChest == null) return;

        IList<ItemModel> targetArray = isChestSlot ? _currentChest.ChestInventory : InventoryManager.Inst.GetMainInventory();
        ItemModel clickedItem = targetArray[slotIndex];

        if (_itemInMouse != null && clickedItem != null && _itemInMouse.ItemDataId == clickedItem.ItemDataId)
        {
            // (여기에 개수 더하는 로직 추가 가능)
        }
        else
        {
            // 마우스가 들고 있던 것과 칸에 있던 것을 맞바꿉니다.
            targetArray[slotIndex] = _itemInMouse;
            _itemInMouse = clickedItem;
        }

        RefreshAllSlot();
    }

    private void RefreshAllSlot()
    {
        if (_currentChest == null) return;

        for (int i = 0; i < _chestSlotList.Count; i++)
        {
            var itemData = _currentChest.ChestInventory[i];

            _chestSlotList[i].UpdateSlot(itemData);
        }
    }
}
