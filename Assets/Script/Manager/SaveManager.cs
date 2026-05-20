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
        Debug.LogWarning("[FarmSaveManager] 세이브 파일 없음 — 새 데이터 생성");
        return GetDefaultSaveData();
    }

    private SaveModel GetDefaultSaveData() => new SaveModel();
}
