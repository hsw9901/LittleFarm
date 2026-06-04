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
                Debug.Log($"{data.Name} {count}개 구매 완료!");
            }
            else
            {
                Debug.Log("구매 실패: 돈이 부족합니다.");
            }
        }
    }

    
    public void SellItem(int inventoryIndex, int count)
    {
        ItemModel myItem = InventoryManager.Inst.GetInventoryItem(inventoryIndex);

        if (myItem != null && !string.IsNullOrEmpty(myItem.ItemDataId))
        {
            if (GameDataManager.Inst.ItemDataList.TryGetValue(myItem.ItemDataId, out ItemData data))
            {
                int totalEarn = data.SellPrice * count;

                InventoryManager.Inst.ConsumeItem(inventoryIndex, count);

                InventoryManager.Inst.AddGold(totalEarn);
                Debug.Log($"{data.Name} {count}개 판매 완료! (+{totalEarn}G)");
            }
        }
    }
}