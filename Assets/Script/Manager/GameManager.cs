using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    [Serializable]
    public struct StarterItem
    {
        public string ItemId; 
        public int Amount; 
    }
    [Header("시작 아이템 설정")]
    [SerializeField] 
    private List<StarterItem> defaultItems = new List<StarterItem>();

    private PlayerModel _playerModel;
    public PlayerModel PlayerData => _playerModel;
    public Player MainPlayer { get; private set; }

    private void Awake()
    {
        Inst = this;
        DontDestroyOnLoad(gameObject);
        InitNewGame();
    }
    
    private async void Start()
    {
        if (GameDataManager.Inst != null)
        {
            await GameDataManager.Inst.InitAllDataAsync();
        }
        int timeoutCount = 0;
        while (InventoryManager.Inst == null && timeoutCount < 100)
        {
            await UniTask.Yield();
            timeoutCount++;
        }
        RequestMainMenu();

        UIManager.Inst.ShowStartupUIOnGameStart();
    }
    public void StartNewGame(string playerName, string worldName)
    {
        _playerModel.PlayerName = playerName;
        _playerModel.WorldName = worldName;

        if (CheckIsNewGame())
        {
            GiveDefaultItems();
        }

        UIManager.Inst.CloseContentUI(UIType.RobbyUI);

        RequestStartGame();
    }
    public void StartLoadedGame(int slotIndex)
    {
        Debug.Log($"[GameManager] {slotIndex}번 세이브 슬롯 로드 시작!");
        
        LoadGame(slotIndex); 

        UIManager.Inst.CloseContentUI(UIType.RobbyUI);
        UIManager.Inst.OpenContentUI(UIType.HudUI);

        RequestStartGame();
    }

    public void InitNewGame()
    {
        _playerModel = new PlayerModel();
        Debug.Log("[GameManager] 새로운 플레이어가 생성되었습니다.");
    }

    public void SaveAndEndGame()
    {
        SaveManager.Inst.SaveGameFlow();
        Application.Quit();
    }
    public void LoadGame(int slotIndex)
    {
        SaveManager.Inst.LoadGameFlow(slotIndex);
    }
    public PlayerModel PackingState()
    {
        if (InventoryManager.Inst != null)
        {
            _playerModel.Inventory = InventoryManager.Inst.GetMainInventory();
        }

        return _playerModel;
    }

    public void ReloadState(PlayerModel data)
    {
        _playerModel = data;

        InventoryManager.Inst.ApplySaveData(_playerModel.Inventory, new ItemModel[10], _playerModel.Gold);
    }

    public void RequestStartGame()
    {
        GameStateManager.Inst.ChangeState(GameState.Playing);
    }

    public void RequestPauseToggle()
    {
        if (GameStateManager.Inst.IsPlaying)
        {
            GameStateManager.Inst.ChangeState(GameState.Paused);
        }

        else if (GameStateManager.Inst.IsPaused)
        {
            GameStateManager.Inst.ChangeState(GameState.Playing);
        }
    }
    public void RequestMainMenu()
    {
        GameStateManager.Inst.ChangeState(GameState.MainMenu);
    }

    private bool CheckIsNewGame()
    {
        return true;
    }

    private void GiveDefaultItems()
    {
        Debug.Log("[GameManager] 기본 아이템 지급");

        if (InventoryManager.Inst == null) return;

        foreach (var item in defaultItems)
        {
            if (!string.IsNullOrEmpty(item.ItemId) && item.Amount > 0)
            {
                InventoryManager.Inst.AddItem(item.ItemId, item.Amount);
            }
        }
    }
    public void RegisterPlayer(Player player)
    {
        MainPlayer = player;
    }
}   
