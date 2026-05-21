using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class SaveModel 
{
    public TimeSaveData TimeData;
}
[Serializable]
public class TimeSaveData
{
    public int Season = 1;
    public int Day = 1;
    public int Hour = 6;
    public int Minute = 0;
}
[Serializable]
public class PlayerSaveData
{
    public Vector3 Position;
}
[Serializable]
public class FarmSaveData
{
    public List<FarmTileData> FarmTileList;
}

