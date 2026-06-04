using UnityEngine;
using System.Collections.Generic;

public class ShopUI : UIBase
{
    [Header("슬롯 생성 설정")]
    public Transform SlotContentRoot; 
    public GameObject ShopSlotPrefab;

    public void InitializeShop(List<string> itemsForSale)
    {
        foreach (Transform child in SlotContentRoot)
        {
            Destroy(child.gameObject);
        }

        foreach (string itemId in itemsForSale)
        {
            GameObject slotObj = Instantiate(ShopSlotPrefab, SlotContentRoot);
            ShopSlotUI slotUI = slotObj.GetComponent<ShopSlotUI>();

            if (slotUI != null)
            {
                slotUI.InitSlot(itemId);
            }
        }
    }

    public void OnClickCloseButton()
    {
        UIManager.Inst.CloseContentUI(UIType.ShopUI);
    }
}