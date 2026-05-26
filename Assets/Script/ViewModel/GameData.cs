using System;
using System.Collections.Generic;


[Serializable]
public class GameDataBase
{
    public string Id;
}
[Serializable]
public class ItemData : GameDataBase
{
    public string Name;
    public ItemType ItemType;
    public int MaxStack;
    public string IconKey;
    public string Description;
}

[Serializable]
public class FieldObjectData
{
    public string Name;
    public FieldObjectType ObjectType;
    public string PrefabKey;
    public bool IsInteractable;
}
