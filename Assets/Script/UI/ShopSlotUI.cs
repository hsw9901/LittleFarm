using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

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

            SetIcon(itemDataId).Forget();

            BuyButton.onClick.RemoveAllListeners();
            BuyButton.onClick.AddListener(OnClickBuy);
        }
    }

    private void OnClickBuy()
    {
        ShopManager.Inst.BuyItem(_currentItemId, 1);
    }

    private async UniTaskVoid SetIcon(string itemDataId)
    {
        if (!GameDataManager.Inst.ItemDataList.TryGetValue(itemDataId, out ItemData data)) return;

        string key = data.IconKey;

        Sprite loadedSprite = await ResourceManager.Inst.LoadAsset<Sprite>(key);

        if (this == null || ItemIcon == null) return;

        if (loadedSprite != null)
        {
            ItemIcon.sprite = loadedSprite;
            ItemIcon.gameObject.SetActive(true); 
        }
    }
}