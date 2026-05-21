using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Inst { get; private set; }

    [Header("타일맵 레이어")]
    [SerializeField] private Tilemap GroundLayer;
    [SerializeField] private Tilemap FarmLayer;
    [SerializeField] private Tilemap CropLayer;

    [Header("타일 애셋")]
    [SerializeField] private TileBase TilledTile;

    private Dictionary<Vector2Int, FarmTileData> _tiles = new();

    private void Start()
    {
        TimeManager.Inst.OnDayChanged += ProcessNextDay;
    }
    private void Awake()
    {
        if (Inst != null) return;
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