public enum ToolType
{
    None = 0,
    Hand,
    Hoe,
    Wateringcan
}

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
public enum GameState
{
    MainMenu,
    Loading,
    Playing,
    Paused
}

public enum Season
{
    Spring = 1,
    Summer,
    Autumn,
    Winter
}
public enum Player_AnimState
{
    None = 0,
    Idle,
    Walk,
    Run
}

public enum ItemType
{
    None = 0,
    Tool,
    Seed,
    Crop,
    Food,
    Furniture
}
public enum FieldObjectType
{
    None = 0,
    Crop
}

public enum ItemUseType
{
    None = 0,
    Tool,
    Consumeable,
    Interactable
}

public enum ItemUseResult
{
    None = 0,
    Consume,
    Keep,
    Fail
}

public enum ToolCategory
{
    None = 0,
    Axe,
    Pickaxe,
    Hoe
}