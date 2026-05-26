using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    [SerializeField] private string[] defaultItemIds = { "Item_Tool_Hoe_01", "Item_Tool_Wateringcan_01"};
    private PlayerModel _playerModel;
    public PlayerModel PlayerData => _playerModel;


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
        
        bool isNewGame = CheckIsNewGame();

        if (isNewGame)
        {
            GiveDefaultItems();
        }
    }

    public void InitNewGame()
    {
        _playerModel = new PlayerModel();
        Debug.Log("[GameManager] 새로운 플레이어가 생성되었습니다.");
    }

    public void SaveGame()
    {
        SaveManager.Inst.SaveGameFlow();
    }
    public void LoadGame()
    {
        SaveManager.Inst.LoadGameFlow();
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

        InventoryManager.Inst.ApplySaveData(_playerModel.Inventory, new ItemModel[6]);
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

        foreach (string itemId in defaultItemIds)
        {
            InventoryManager.Inst.AddItem(itemId, 1);
        }
    }
}   
