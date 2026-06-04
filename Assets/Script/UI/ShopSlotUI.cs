using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlotUI : MonoBehaviour
{
    [Header("UI 연결")]
    public Image ItemIcon;             
    public TextMeshProUGUI NameText;   
    public TextMeshProUGUI PriceText;  
    public Button BuyButton;           

    private string _currentItemId;

    public void InitSlot(string itemDataId)
    {
        _currentItemId = itemDataId;

        if (GameDataManager.Inst.ItemDataList.TryGetValue(itemDataId, out ItemData data))
        {
            NameText.text = data.Name;
            PriceText.text = $"{data.BuyPrice} G";


            BuyButton.onClick.RemoveAllListeners();
            BuyButton.onClick.AddListener(OnClickBuy);
        }
    }

    private void OnClickBuy()
    {
        ShopManager.Inst.BuyItem(_currentItemId, 1);
    }
}