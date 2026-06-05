using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerModel
{
    public string PlayerName = "새농부";
    public string WorldName = "";
    public int Gold = 500;
    public int CurrentStamina = 100;
    public int MaxStamina = 100;
    public float PosX;
    public float PosY;
    public float PosZ;

    public List<ItemModel> Inventory = new List<ItemModel>();
    
    public PlayerModel()
    {
        int maxSlots = 36;

        for (int i = 0; i < maxSlots; i++)
        {
            Inventory.Add(new ItemModel { ItemDataId = "", ItemStackCount = 0 });
        }
    }
}