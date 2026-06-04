using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Inst { get; private set; }

    private void Awake()
    {
        Inst = this;
    }

    public void BuyItem(string itemDataId, int count)
    {
        if (GameDataManager.Inst.ItemDataList.TryGetValue(itemDataId, out ItemData data))
        {
            int totalPrice = data.BuyPrice * count;

            if (InventoryManager.Inst.TryUseGold(totalPrice))
            {
                InventoryManager.Inst.AddItem(itemDataId, count);
                UIManager.Inst.OpenSimplePopup($"<color=green>{data.Name}</color> {count}개 구매 완료");
            }
            else
            {
                UIManager.Inst.OpenSimplePopup("구매 실패: 돈이 부족합니다");
            }
        }
    }
    
    public void SellItem(InventoryManager.SlotArea area, int slotIndex, int count)
    {
        ItemModel myItem = InventoryManager.Inst.GetItemByArea(area, slotIndex);

        if (myItem != null && !string.IsNullOrEmpty(myItem.ItemDataId))
        {
            if (myItem.ItemStackCount < count)
            {
                UIManager.Inst.OpenSimplePopup("아이템 개수가 부족합니다");
                return;
            }

            if (GameDataManager.Inst.ItemDataList.TryGetValue(myItem.ItemDataId, out ItemData data))
            {
                int totalEarn = data.SellPrice * count;

                InventoryManager.Inst.ConsumeItemByArea(area, slotIndex, count);

                InventoryManager.Inst.AddGold(totalEarn);

                UIManager.Inst.OpenSimplePopup($"<color=green>{data.Name}</color> 판매 완료! <color=yellow>+{totalEarn}G</color>");
            }
        }
    }
}