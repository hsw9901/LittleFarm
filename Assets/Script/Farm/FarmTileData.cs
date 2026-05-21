using System;
using UnityEngine;



[Serializable]
public class FarmTileData
{
    public Vector2Int GridPos;
    public TileState state = TileState.Empty;
    public int Moisture = 0;
    public string CropId = "";
    public int DaysGrown = 0;
    public bool CanTill => state == TileState.Empty;
    public bool CanPlant => state == TileState.Tilled || state == TileState.Watered;
    public bool CanHarvest => state == TileState.CanHarvest;
    public bool IsWatered => Moisture > 0;
}