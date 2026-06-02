using UnityEngine;
using System;

public enum UIRootType
{
    None = 0,
    BackgroundUI,
    MainUI,
    ContentUI,
    PopupUI,
    VeryFrontUI
}

public enum UIType
{
    ProfilePopup,
    Inventory,
    ChestUI,
    HudUI,
    TimeUI
}

public static class UIManagerExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRootType uiRootType, UIType uiType)
    {
        string path = string.Empty;

        path = $"Prefab/UI/{uiRootType}/{uiType}";
        
        return path;
    }

    public static void OpenInventoryPopup(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenContentUI(UIType.Inventory);
        if (uiBase == null) 
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static void CloseInventoryPopup(this UIManager uiManager)
    {
        uiManager.CloseContentUI(UIType.Inventory);
        uiManager.CloseContentUI(UIType.ChestUI);
    }

    public static void OpenChestPopup(this UIManager uiManager, Chest chestData)
    {
        if (uiManager.IsOpened(UIType.ProfilePopup))
        {
            uiManager.CloseContentUI(UIType.ProfilePopup);
        }

        if (!uiManager.IsOpened(UIType.Inventory))
        {
            uiManager.OpenContentUI(UIType.Inventory);
        }

        var uiBase = uiManager.OpenContentUI(UIType.ChestUI);

        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }

        if (uiBase is ChestController chestUI)
        {
            chestUI.OpenChestUI(chestData);
        }
    }

    public static void ToggleInventoryPopup(this UIManager uiManager)
    {
        if (uiManager.IsOpened(UIType.Inventory) || uiManager.IsOpened(UIType.ChestUI))
        {
            uiManager.CloseContentUI(UIType.Inventory);
            uiManager.CloseContentUI(UIType.ProfilePopup);
            uiManager.CloseContentUI(UIType.ChestUI);
        }
        else
        {
            uiManager.OpenContentUI(UIType.Inventory);
            uiManager.OpenContentUI(UIType.ProfilePopup);
        }
    }

    public static void OpenTimeUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenUI(UIRootType.MainUI, UIType.TimeUI);

        if (uiBase == null)
        {
            Debug.LogWarning("UI가 생성되지 않았습니다");
        }
    }

}
