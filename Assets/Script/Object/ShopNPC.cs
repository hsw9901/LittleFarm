using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    [Header("상점 판매 상품")]
    public List<string> ItemsForSale;

    public void OpenShop()
    {
        UIManager.Inst.OpenSimplePopup("상점에 어서오세요");

        if (UIManager.Inst != null)
        {
            UIManager.Inst.OpenShopUI(ItemsForSale); 
        }
    }
}