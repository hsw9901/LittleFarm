using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    private PlayerModel _playerModel;
    public PlayerModel PlayerData => _playerModel;


    private void Awake()
    {
        Inst = this;
        DontDestroyOnLoad(gameObject);
        InitNewGame();
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
}   
