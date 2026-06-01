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
        Debug.Log($"[{itemDataId}]의 현재 UseType 값은 [{itemData.ItemUseType}] 입니다!");
        Player currentPlayer = GameManager.Inst.MainPlayer;
        Vector2Int targetPos = currentPlayer.GetTargetGridPosition();

        if (itemData.GetItemUseType() == ItemUseType.Tool)
        {
            int currentToolPower = 1;
            if (GameDataManager.Inst.ToolDataList.TryGetValue(itemDataId, out var toolData))
            {
                currentToolPower = toolData.ToolPower;
            }
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
                case "Item_Tool_Axe_01":
                    if (GameManager.Inst != null && GameManager.Inst.MainPlayer != null)
                    {
                        GameManager.Inst.MainPlayer.HitFieldResource(ToolCategory.Axe, currentToolPower);
                    }
                    Debug.Log($"도끼를 사용했습니다! (적용된 파워: {currentToolPower})");
                    break;
                case "Item_Tool_Pickaxe_01":
                    if (GameManager.Inst != null && GameManager.Inst.MainPlayer != null)
                    {
                        GameManager.Inst.MainPlayer.HitFieldResource(ToolCategory.Pickaxe, currentToolPower);
                    }
                    Debug.Log($"곡괭이를 사용했습니다! (적용된 파워: {currentToolPower})");
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
                    bool isPlanted = FarmManager.Inst.RequestPlantSeed(targetPos, "Item_Seed_Tomato", 3);
                    if (isPlanted)
                    {
                        Debug.Log("토마토씨앗심기");
                        return ItemUseResult.Consume;
                    }
                    else
                    {
                        return ItemUseResult.Fail;
                    }
            }
            return ItemUseResult.Consume;
        }

        return ItemUseResult.Fail;
    }
}
