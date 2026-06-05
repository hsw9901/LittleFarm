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
        return Path.Combine(Application.persistentDataPath, "SaveData_{slotIndex}.json");
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
        currentSave.TimeData = TimeManager.Inst.PackingState();
        currentSave.FarmData = FarmManager.Inst.PackingState();
        currentSave.PlayerData = GameManager.Inst.PackingState();
        RequestSaveData(currentSave, CurrentSlotIndex);
    }
    public void LoadGameFlow(int slotIndex)
    {
        SaveModel loadedData = RequestLoadSaveData(slotIndex);
        if (loadedData == null) return;
        TimeManager.Inst.ReloadState(loadedData.TimeData);
        FarmManager.Inst.ReloadState(loadedData.FarmData);
        GameManager.Inst.ReloadState(loadedData.PlayerData);
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
}
