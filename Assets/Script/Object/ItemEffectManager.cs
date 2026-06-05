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
                    UIManager.Inst.OpenSimplePopup("밭을 갈았습니다.");
                    Debug.Log("밭을 갈았습니다!");
                    break;
                case "Item_Tool_Wateringcan_01":
                    FarmManager.Inst.RequestWaterTile(targetPos);
                    UIManager.Inst.OpenSimplePopup("물을 주었습니다.");
                    Debug.Log("물을 주었습니다!");
                    break;
                case "Item_Tool_Axe_01":
                    if (GameManager.Inst != null && GameManager.Inst.MainPlayer != null)
                    {
                        GameManager.Inst.MainPlayer.HitFieldResource(ToolCategory.Axe, currentToolPower);
                    }
                    UIManager.Inst.OpenSimplePopup($"도끼를 사용했습니다 (적용된 파워: {currentToolPower})");
                    Debug.Log($"도끼를 사용했습니다! (적용된 파워: {currentToolPower})");
                    break;
                case "Item_Tool_Pickaxe_01":
                    if (GameManager.Inst != null && GameManager.Inst.MainPlayer != null)
                    {
                        GameManager.Inst.MainPlayer.HitFieldResource(ToolCategory.Pickaxe, currentToolPower);
                    }
                    UIManager.Inst.OpenSimplePopup($"곡괭이를 사용했습니다 (적용된 파워: {currentToolPower})");
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
            if (itemDataId.Contains("Seed"))
            {
                bool isPlanted = FarmManager.Inst.RequestPlantSeed(targetPos, itemDataId, 3);

                if (isPlanted)
                {
                    Debug.Log($"{itemDataId} 심기 성공!");
                    return ItemUseResult.Consume;
                }
                else
                {
                    UIManager.Inst.OpenSimplePopup("씨앗을 심을 수 없는 땅입니다.");
                    Debug.Log("씨앗을 심을 수 없는 땅입니다.");
                    return ItemUseResult.Fail;
                }
            }

            switch (itemDataId)
            {
                case "Item_Crop_Tomato":
                    Debug.Log("스테미나를 회복했습니다");
                    return ItemUseResult.Consume;
            }
        }
        return ItemUseResult.Fail;
    }
}
