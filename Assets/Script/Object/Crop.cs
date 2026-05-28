using UnityEngine;

public class Crop : MonoBehaviour
{
    [Header("작물 정보")]
    public string CropId;
    public string HarvestItemId;
    public int CurrentGrowthDay;
    public int MaxGrowthDay;

    [Header("상태")]
    public bool IsWatered;
    public bool IsReadyToHarvest;

    private SpriteRenderer _spriteRenderer;
    private Sprite[] _mySprites; 

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitCrop(string cropId, int maxGrowthDay)
    {
        CropId = cropId;
        CurrentGrowthDay = 0; 
        MaxGrowthDay = maxGrowthDay;
        IsWatered = false;
        IsReadyToHarvest = false;

        var data = GameDataManager.Inst.GetCropData(cropId);
        if (data != null)
        {
            _mySprites = data.GrowthSprites;
            HarvestItemId = data.HarvestItemId;
        }

        UpdateSprite();
    }

    public void WaterCrop()
    {
        if (IsWatered) return;

        IsWatered = true;
    }

    public void GrowNextDay()
    {
        if (!IsWatered) return; 

        CurrentGrowthDay++;
        IsWatered = false; 

        if (CurrentGrowthDay >= MaxGrowthDay)
        {
            CurrentGrowthDay = MaxGrowthDay;
            IsReadyToHarvest = true;
        }

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (_mySprites == null || _mySprites.Length == 0) return;

        int spriteIndex = Mathf.Min(CurrentGrowthDay, _mySprites.Length - 1);

        // 최대 성장일(수확 가능 상태)에 도달하면 무조건 마지막 이미지를 강제 적용합니다.
        if (CurrentGrowthDay >= MaxGrowthDay)
        {
            spriteIndex = _mySprites.Length - 1;
        }

        _spriteRenderer.sprite = _mySprites[spriteIndex];
    }
}