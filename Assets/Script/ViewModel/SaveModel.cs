using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class SaveModel 
{
    public TimeSaveData TimeData = new TimeSaveData();
    public PlayerModel PlayerData = new PlayerModel();
    public FarmSaveData FarmData = new FarmSaveData();
    public InventorySaveData InventoryData = new InventorySaveData();
}

[Serializable]
public class TimeSaveData
{
    public int Season;
    public int Day = 1;
    public int Hour = 6;
    public int Minute = 0;
}

[Serializable]
public class FarmSaveData
{
    public List<FarmTileData> FarmTileList;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemModel> MainInventoryItems = new List<ItemModel>();
    public ItemModel[] HotbarItems = new ItemModel[10];
    public int CurrentGold = 0;
}
