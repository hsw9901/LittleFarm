using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Inst { get; private set; }

    [Header("타일맵 레이어")]
    [SerializeField] private Tilemap GroundLayer;
    [SerializeField] private Tilemap FarmLayer;
    [SerializeField] private Tilemap HighlightLayer;

    [Header("타일 애셋")]
    [SerializeField] private TileBase TilledTile;
    [SerializeField] private TileBase WetTilledTile;
    [SerializeField] private TileBase HighlightTile;

    [Header("작물 설정")]
    [SerializeField] private GameObject CropPrefab;

    private Dictionary<Vector2Int, FarmTileData> _tiles = new();
    private Dictionary<Vector2Int, Crop> _cropInstances = new();
    
    private void Awake()
    {
        Inst = this;
    }
    private void Start()
    {
        TimeManager.Inst.OnDayChanged += ProcessNextDay;
    }
    public FarmSaveData PackingState()
    {
        return new FarmSaveData { FarmTileList = new List<FarmTileData>(_tiles.Values) };
    }

    public void ReloadState(FarmSaveData data)
    {
        if(data == null || data.FarmTileList == null) {  return; }

        _tiles.Clear();
        FarmLayer.ClearAllTiles();

        foreach (var crop in _cropInstances.Values) { Destroy(crop.gameObject); }
        _cropInstances.Clear();

        foreach (var tileData in data.FarmTileList)
        {
            _tiles[tileData.GridPos] = tileData;

            if (tileData.state == TileState.Tilled)
            {
                FarmLayer.SetTile((Vector3Int)tileData.GridPos, TilledTile);
            }
            else if (tileData.state == TileState.Watered)
            {
                FarmLayer.SetTile((Vector3Int)tileData.GridPos, WetTilledTile);
            }

            if (!string.IsNullOrEmpty(tileData.CropId))
            {
                SpawnCropObject(tileData.GridPos, tileData);
            }
        }
    }

    // 호미 사용
    public void RequestTillTile(Vector2Int pos)
    {
        // 땅 타일이 있는지 확인
        if (GroundLayer.GetTile((Vector3Int)pos) == null)
        {
            Debug.Log("경작할 수 없는 땅입니다.");
            return;
        }

        var tile = GetOrCreateTile(pos);

        if (tile.CanTill == false)
        {
            Debug.Log("이미 경작된 땅입니다.");
            return;
        }

        tile.state = TileState.Tilled;
        FarmLayer.SetTile((Vector3Int)pos, TilledTile);
        Debug.Log($"[FarmManager] {pos} 경작 완료");
    }

    // 물뿌리개 사용
    public void RequestWaterTile(Vector2Int pos)
    {
        if (_tiles.TryGetValue(pos, out FarmTileData tile))
        {
            if (tile.state == TileState.Empty) return;

            tile.Moisture = 1;

            if (tile.state == TileState.Tilled || tile.state == TileState.Growing) 
            {
                tile.state = TileState.Watered;
            }
            // 경작지를 젖은 경작지 타일로 변경
            FarmLayer.SetTile((Vector3Int)pos, WetTilledTile);

            Debug.Log($"{pos}타일에 물을 주었습니다.");

            if (_cropInstances.TryGetValue(pos, out var crop)) 
            {
                crop.WaterCrop();
            }
        }
    }

    public bool RequestPlantSeed(Vector2Int pos, string cropId, int maxGrowthDay = 3)
    {
        if (!_tiles.TryGetValue(pos, out FarmTileData tile)) { return false; }

        if (tile.state != TileState.Tilled && tile.state != TileState.Watered) { return false; }

        if (!string.IsNullOrEmpty(tile.CropId)) { return false; }

        tile.CropId = cropId;
        tile.DaysGrown = 0;
        tile.state = (tile.state == TileState.Watered) ? TileState.Watered : TileState.Growing;

        SpawnCropObject(pos, tile, maxGrowthDay);
        return true;
    }

    public void SpawnCropObject(Vector2Int pos, FarmTileData tileData, int maxGrowthDay = 3)
    {
        Vector3 worldPos = GroundLayer.GetCellCenterWorld((Vector3Int)pos);

        GameObject cropObj = Instantiate(CropPrefab, worldPos, Quaternion.identity);

        if (cropObj.TryGetComponent(out Crop cropComponent))
        {
            cropComponent.InitCrop(tileData.CropId, maxGrowthDay);

            cropComponent.CurrentGrowthDay = tileData.DaysGrown;
            if (tileData.state == TileState.Watered) cropComponent.WaterCrop();

            _cropInstances[pos] = cropComponent;
        }
    }

    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        Vector3Int cellPos = GroundLayer.WorldToCell(worldPos);
        return (new Vector2Int(cellPos.x, cellPos.y));
    }

    private FarmTileData GetOrCreateTile(Vector2Int pos)
    {
        if (_tiles.ContainsKey(pos) == false)
            _tiles.Add(pos, new FarmTileData { GridPos = pos });

        return _tiles[pos];
    }

    private void ProcessNextDay()
    {
        foreach (var kvp in _tiles) 
        { 
            FarmTileData tile = kvp.Value;

            if(tile.state == TileState.Watered)
            {
                tile.Moisture = 0;
                // 하루가 흐르면 마른 땅으로 변경
                if (string.IsNullOrEmpty(tile.CropId))
                {
                    tile.state = TileState.Tilled;
                    FarmLayer.SetTile((Vector3Int)tile.GridPos, TilledTile);
                }
                else
                {
                    tile.DaysGrown++;
                    tile.state = TileState.Growing;

                    FarmLayer.SetTile((Vector3Int)tile.GridPos, TilledTile);

                    if (_cropInstances.TryGetValue(tile.GridPos, out Crop crop))
                    {
                        crop.GrowNextDay();
                    }
                }
            }
            else if (tile.state == TileState.Tilled)
            {
                //물이 없는 타일의 작물은 성장하지 않음
            }
        }
    }

    public bool RequestHarvest(Vector2Int pos)
    {
        if (!_tiles.TryGetValue(pos, out FarmTileData tile)) return false;
        if (!_cropInstances.TryGetValue(pos, out Crop crop)) return false;

        if (crop.IsReadyToHarvest)
        {
            string harvestItemId = crop.HarvestItemId;

            if (!string.IsNullOrEmpty(harvestItemId))
            {
                InventoryManager.Inst.AddItem(harvestItemId, 1);
                Debug.Log($"{pos}에서 {harvestItemId}를 수확했습니다!");
            }

            Destroy(crop.gameObject);
            _cropInstances.Remove(pos);

            tile.CropId = "";
            tile.DaysGrown = 0;
            tile.state = (tile.Moisture > 0) ? TileState.Watered : TileState.Tilled;

            return true;
        }
        Debug.Log("아직 수확할 수 없습니다!");
        return false;
    }
}