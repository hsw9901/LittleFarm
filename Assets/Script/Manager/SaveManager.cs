using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Inst {  get; private set; }
    public int CurrentSlotIndex { get; private set; } = 1;

    private void Awake()
    {
        Inst = this;
    }

    private string GetPath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"SaveData_{slotIndex}.json");
    }

    public bool HasSaveData(int slotIndex)
    {
        return File.Exists(GetPath(slotIndex));
    }

    public void RequestSaveData(SaveModel data, int slotIndex)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(slotIndex), json);
        Debug.Log($"[SaveManager] 저장 완료: {GetPath(slotIndex)}");
    }

    public SaveModel RequestLoadSaveData(int slotIndex)
    {
        string path = GetPath(slotIndex);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log($"[SaveManager] {slotIndex}번 슬롯 데이터 로드 완료");
            return JsonUtility.FromJson<SaveModel>(json);
        }
        Debug.LogWarning($"[SaveManager] {slotIndex}번 세이브 파일 없음 — 새 데이터 생성");
        return GetDefaultSaveData();
    }

    private SaveModel GetDefaultSaveData() => new SaveModel();

    public void SaveGameFlow()
    {
        SaveModel currentSave = new SaveModel();
        if (TimeManager.Inst != null)
        {
            currentSave.TimeData = TimeManager.Inst.PackingState();
        }
        if (FarmManager.Inst != null)
        {
            currentSave.FarmData = FarmManager.Inst.PackingState();
        }
        if (GameManager.Inst != null) 
        {
            currentSave.PlayerData = GameManager.Inst.PackingState(); 
        }
        if (InventoryManager.Inst != null) 
        {
            currentSave.InventoryData = InventoryManager.Inst.PackingState(); 
        }
        RequestSaveData(currentSave, CurrentSlotIndex);
    }
    public void LoadGameFlow(int slotIndex)
    {
        SaveModel loadedData = RequestLoadSaveData(slotIndex);
        if (loadedData == null) { return; }

        if (TimeManager.Inst != null)
        {
            TimeManager.Inst.ReloadState(loadedData.TimeData);
        }
        if (FarmManager.Inst != null)
        {
            FarmManager.Inst.ReloadState(loadedData.FarmData);
        }
        if (GameManager.Inst != null)
        {
            GameManager.Inst.ReloadState(loadedData.PlayerData);
        }

        if (InventoryManager.Inst != null)
        {
            InventoryManager.Inst.ReloadState(loadedData.InventoryData);
        }
    }
    public SaveModel GetSavePreview(int slotIndex)
    {
        string path = GetPath(slotIndex);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveModel>(json);
        }
        return null;
    }

    public void FindEmptySlotForNewGame() 
    {
        for (int i = 1; i <= 5; i++)
        {
            if (!HasSaveData(i))
            {
                CurrentSlotIndex = i;
                return;
            }
        }
        
        Debug.LogWarning("[SaveManager] 모든 슬롯이 꽉 찼습니다!");
        return; 
    }
    public void SetCurrentSlot(int index)
    {
        CurrentSlotIndex = index;
    }
}
