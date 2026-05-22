using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//using static UnityEditor.PlayerSettings;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Inst { get; private set; }

    [Header("타일맵 레이어")]
    [SerializeField] private Tilemap GroundLayer;
    [SerializeField] private Tilemap FarmLayer;
    [SerializeField] private Tilemap HighlightLayer;
    [SerializeField] private Tilemap CropLayer;

    [Header("타일 애셋")]
    [SerializeField] private TileBase TilledTile;
    [SerializeField] private TileBase WetTilledTile;
    [SerializeField] private TileBase HighlightTile;


    private Dictionary<Vector2Int, FarmTileData> _tiles = new();

    private void Start()
    {
        TimeManager.Inst.OnDayChanged += ProcessNextDay;
    }
    private void Awake()
    {
        Inst = this;
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

            if (tile.state == TileState.Tilled) 
            {
                tile.state = TileState.Watered;
            }
            // 경작지를 젖은 경작지 타일로 변경
            FarmLayer.SetTile((Vector3Int)pos, WetTilledTile);

            Debug.Log($"{pos}타일에 물을 주었습니다.");
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
                tile.state = TileState.Tilled;
                FarmLayer.SetTile((Vector3Int)tile.GridPos, TilledTile);


                if (!string.IsNullOrEmpty(tile.CropId))
                {
                    tile.DaysGrown++;
                }

                FarmLayer.SetTile((Vector3Int)tile.GridPos, TilledTile);
                tile.state = TileState.Tilled;
            }
            else if (tile.state == TileState.Tilled)
            {
                //물이 없는 타일의 작물은 성장하지 않음
            }
        }
    }
}