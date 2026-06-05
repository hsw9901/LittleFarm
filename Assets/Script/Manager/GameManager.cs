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
        SaveManager.Inst.FindEmptySlotForNewGame();

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

        SaveManager.Inst.SetCurrentSlot(slotIndex);
        LoadGame(slotIndex); 

        UIManager.Inst.CloseContentUI(UIType.RobbyUI);

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

    public void SaveAndReturnToLobby()
    {
        SaveManager.Inst.SaveGameFlow();
        Debug.Log("[GameManager] 게임 저장 완료! 로비로 돌아갑니다.");

        UIManager.Inst.CloseAllOpenedPopup();
        RequestMainMenu();

        
        if (UIManager.Inst != null)
        {
            UIManager.Inst.OpenContentUI(UIType.RobbyUI);
        }
    }
    public void LoadGame(int slotIndex)
    {
        SaveManager.Inst.LoadGameFlow(slotIndex);
    }
    public PlayerModel PackingState()
    {
        if (MainPlayer != null)
        {
            _playerModel.PosX = MainPlayer.transform.position.x;
            _playerModel.PosY = MainPlayer.transform.position.y;
            _playerModel.PosZ = MainPlayer.transform.position.z;
        }

        if (InventoryManager.Inst != null)
        {
            _playerModel.Inventory = InventoryManager.Inst.GetMainInventory();
        }

        return _playerModel;
    }

    public void ReloadState(PlayerModel data)
    {
        _playerModel = data;
        MovePlayerToSavedPosition();
        if (InventoryManager.Inst != null)
        {
            InventorySaveData invData = new InventorySaveData();
            invData.MainInventoryItems = _playerModel.Inventory;
            invData.CurrentGold = _playerModel.Gold;

            InventoryManager.Inst.ReloadState(invData);
        }
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
    public void MovePlayerToSavedPosition()
    {
        if (MainPlayer != null && _playerModel != null)
        {
            MainPlayer.transform.position = new Vector3(_playerModel.PosX, _playerModel.PosY, _playerModel.PosZ);
        }
    }
}   
