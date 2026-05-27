using UnityEngine;

public class ItemEffectManager : MonoBehaviour
{
    public static ItemEffectManager Inst {  get; private set; }

    private void Awake()
    {
        Inst = this;
    }

    public ItemUseResult ApplyEffect(string itemDataId)
    {
        if (!GameDataManager.Inst.ItemDataList.TryGetValue(itemDataId, out var itemData))
        {
            return ItemUseResult.Fail;
        }
        Debug.Log($"🚨 디버그: [{itemDataId}]의 현재 UseType 값은 [{itemData.ItemUseType}] 입니다!");

        if (itemData.GetItemUseType() == ItemUseType.Tool)
        {
            Player currentPlayer = GameManager.Inst.MainPlayer;
            Vector2Int targetPos = currentPlayer.GetTargetGridPosition();

            switch (itemDataId)
            {
                case "Item_Tool_Hoe_01": 
                    FarmManager.Inst.RequestTillTile(targetPos);
                    Debug.Log("밭을 갈았습니다!");
                    break;
                case "Item_Tool_Wateringcan_01":
                    FarmManager.Inst.RequestWaterTile(targetPos);
                    Debug.Log("물을 주었습니다!");
                    break;
                default:
                    Debug.LogWarning($"정의되지 않은 도구입니다: {itemDataId}");
                    return ItemUseResult.Fail;
            }

            return ItemUseResult.Keep;
        }
        else if (itemData.GetItemUseType() == ItemUseType.Consumeable)
        {
            switch (itemDataId)
            {
                case "Item_Crop_Tomato":
                    Debug.Log("스테미나를 회복했습니다");
                    break;
                case "Item_Seed_Tomato":
                    Debug.Log("토마토씨앗심기");
                    break;
            }
            return ItemUseResult.Consume;
        }

        return ItemUseResult.Fail;
    }
}
