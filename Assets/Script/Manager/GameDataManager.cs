using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Inst { get; private set; }

    [Header("작물 이미지 데이터")]
    public List<CropSpriteData> CropSprites = new List<CropSpriteData>();

    public Sprite[] GetCropSprites(string cropId)
    {
        var data = CropSprites.Find(x => x.CropId == cropId);
        return data != null ? data.GrowthSprites : null;
    }

    [Serializable]
    private class SerializationWrapper<T>
    {
        public List<T> items;
    }

    public Dictionary<string, ItemData> ItemDataList { get; private set; } = new();

    private void Awake()
    {
        Inst = this;
    }

    public async UniTask InitAllDataAsync()
    {
        Debug.Log("[DataManager] 데이터 로드 시작");

        await UniTask.WhenAll(
            LoadDataAsync<ItemData>("Data_Item").ContinueWith(d => ItemDataList = d)
        );

    }

    private async UniTask<Dictionary<string, T>> LoadDataAsync<T>(string addressableKey) where T : GameDataBase
    {
        TextAsset textAsset = await ResourceManager.Inst.LoadAsset<TextAsset>(addressableKey);

        if (textAsset == null)
        {
            Debug.LogError($"[DataManager] 리소스를 찾을 수 없습니다. Key: {addressableKey}");
            return new Dictionary<string, T>();
        }

        try
        {
            string jsonString = textAsset.text;

            string wrappedJson = "{\"items\":" + jsonString + "}";
            SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);

            if (wrapper != null && wrapper.items != null)
            {
                Debug.Log($"[DataManager] {typeof(T).Name} 데이터를 {wrapper.items.Count}개 로드했습니다.");

                return wrapper.items.ToDictionary(item => item.Id.ToString());
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[DataManager] {typeof(T).Name} JSON 파싱 오류: {ex.Message}");
        }

        return new Dictionary<string, T>();
    }
    public CropSpriteData GetCropData(string cropId)
    {
        return CropSprites.Find(x => x.CropId == cropId);
    }
}
