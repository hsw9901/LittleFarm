using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Inst {  get; private set; }

    private void Awake()
    {
        Inst = this;
    }

    private string GetPath()
    {
        return Path.Combine(Application.persistentDataPath, "SaveData.json");
    }

    public void RequestSaveData(SaveModel data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(), json);
        Debug.Log($"[SaveManager] 저장 완료: {GetPath()}");
    }

    public SaveModel RequestLoadSaveData()
    {
        string path = GetPath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("[FarmSaveManager] 데이터 로드 완료");
            return JsonUtility.FromJson<SaveModel>(json);
        }
        Debug.LogWarning("[SaveManager] 세이브 파일 없음 — 새 데이터 생성");
        return GetDefaultSaveData();
    }

    private SaveModel GetDefaultSaveData() => new SaveModel();

    public void SaveGameFlow()
    {
        SaveModel currentSave = new SaveModel();
        currentSave.TimeData = TimeManager.Inst.PackingState();
        currentSave.FarmData = FarmManager.Inst.PackingState();
        currentSave.PlayerData = GameManager.Inst.PackingState();
        RequestSaveData(currentSave);
    }
    public void LoadGameFlow()
    {
        SaveModel loadedData = RequestLoadSaveData();
        if (loadedData == null) return;
        TimeManager.Inst.ReloadState(loadedData.TimeData);
        FarmManager.Inst.ReloadState(loadedData.FarmData);
        GameManager.Inst.ReloadState(loadedData.PlayerData);
    }
}
