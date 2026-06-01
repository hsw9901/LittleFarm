using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GameDataBase
{
    public string Id;
}
[Serializable]
public class ItemData : GameDataBase
{
    public string Name;
    public string ItemType;
    public string ItemUseType;
    public int MaxStack;
    public string IconKey;
    public string Description;
    public string PrefabKey;
    public int BuyPrice;
    public int SellPrice;
    public ItemType GetItemType()
    {
        if (Enum.TryParse<ItemType>(this.ItemType, true, out ItemType result))
            return result;

        return default;
    }
    public ItemUseType GetItemUseType()
    {
        if (Enum.TryParse<ItemUseType>(this.ItemUseType, true, out ItemUseType result))
            return result;

        return default;
    }
}

[Serializable]
public class FieldObjectData
{
    public string Name;
    public FieldObjectType ObjectType;
    public string PrefabKey;
    public bool IsInteractable;
}

[Serializable]
public class CropSpriteData
{
    public string CropId;
    public string HarvestItemId;
    public Sprite[] GrowthSprites;
}

[Serializable]
public class ToolExpansion : GameDataBase
{
    public string ToolCategory;
    public int StaminaCost;
    public int ToolPower;

    public ToolCategory GetToolCategory()
    {
        if (Enum.TryParse<ToolCategory>(this.ToolCategory, true, out ToolCategory UseTool))
            return UseTool;

        return default;
    }
}