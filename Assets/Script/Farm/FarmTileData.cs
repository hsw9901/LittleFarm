using System;
using UnityEngine;

public enum TileState
{
    None = 0,
    Empty,
    Tilled,
    Watered,
    Seeded,
    Growing,
    CanHarvest
}

[Serializable]
public class FarmTileData
{
    public Vector2Int gridPos;
    public TileState state = TileState.Empty;
    public int moisture = 0;
    public string cropId = "";
    public int daysGrown = 0;
    public bool CanTill => state == TileState.Empty;
    public bool CanPlant => state == TileState.Tilled || state == TileState.Watered;
    public bool CanHarvest => state == TileState.CanHarvest;
    public bool IsWatered => moisture > 0;
}