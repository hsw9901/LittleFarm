using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerModel _playerModel;
    public PlayerModel PlayerData => _playerModel;

    public static GameManager Inst { get; private set; }

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

    public void LoadGameData(PlayerModel loadedPlayerData)
    {
        if (loadedPlayerData == null)
        {
            Debug.LogError("[GameManager] 저장된 플레이어 데이터가 없습니다.");
            return;
        }
        _playerModel = loadedPlayerData;
    }

    public void AddItem(string itemDataId, int amount = 1)
    {
        if (_playerModel == null) { return; }
        ItemModel existingItem = _playerModel.Inventory.Find(item => item.ItemDataId == itemDataId);

        if (existingItem != null)
        {
            existingItem.ItemStackCount += amount;
            return;
        }

        int emptySlotIndex = _playerModel.Inventory.FindIndex(item => string.IsNullOrEmpty(item.ItemDataId));
        
        if (emptySlotIndex != -1)
        {
            _playerModel.Inventory[emptySlotIndex].ItemDataId = itemDataId;
            _playerModel.Inventory[emptySlotIndex].ItemStackCount = amount;
            Debug.Log($"[GameManager] {emptySlotIndex}번 빈 슬롯에 새 아이템 추가: {itemDataId}");
        }
        else
        {
            Debug.LogWarning("[GameManager] 가방이 가득 찼습니다!");
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

    public List<ItemModel> GetPlayerItemList()
    {
        if (_playerModel == null) return new List<ItemModel>();

        return _playerModel.Inventory;
    }

    private ItemModel[] _hotbarItems = new ItemModel[6];

    public ItemModel[] GetHotbarItemList()
    {
        return _hotbarItems;
    }
}
