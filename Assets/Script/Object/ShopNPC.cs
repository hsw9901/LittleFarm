using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    [Header("이 상점에서 파는 아이템들")]
    public List<string> ItemsForSale;

    public void OpenShop()
    {
        Debug.Log("상점에 어서오세요!");

        if (UIManager.Inst != null)
        {
            UIManager.Inst.OpenShopUI(ItemsForSale); 
        }
    }
}