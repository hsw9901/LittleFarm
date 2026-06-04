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
    TimeUI,
    ShopUI,
    RobbyUI,
    SimplePopup,
    GoldUI
}

public static class UIManagerExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRootType uiRootType, UIType uiType)
    {
        string path = string.Empty;

        path = $"Prefab/UI/{uiRootType}/{uiType}";
        
        return path;
    }

    public static void ShowStartupUIOnGameStart(this UIManager uiManager)
    {
        uiManager.OpenContentUI(UIType.RobbyUI);
        uiManager.OpenMainUI(UIType.HudUI);
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
        if (uiManager.IsOpened(UIType.ShopUI) || uiManager.IsOpened(UIType.ChestUI))
        {
            return;
        }
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
    
    public static void OpenShopUI(this UIManager uiManager, System.Collections.Generic.List<string> itemsForSale)
    {
        if (!uiManager.IsOpened(UIType.Inventory))
        {
            uiManager.OpenContentUI(UIType.Inventory);
        }

        var uiBase = uiManager.OpenContentUI(UIType.ShopUI);

        if (uiBase == null)
        {
            Debug.LogWarning("UI가 생성되지 않았습니다");
        }
        if (uiBase is ShopUI shopUI)
        {
            shopUI.InitializeShop(itemsForSale);
        }
    }

    public static void OpenSimplePopup(this UIManager uiManager, string msg)
    {
        var uiBase = uiManager.OpenPopupUI(UIType.SimplePopup);
        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
        if(uiBase is SimplePopup simplePopup)
        {
            simplePopup.SetUI(msg);
        }
    }
    public static void CloseAllOpenedPopup(this UIManager uiManager)
    {
        uiManager.CloseContentUI(UIType.ShopUI);
        uiManager.CloseContentUI(UIType.ChestUI);
        uiManager.CloseContentUI(UIType.Inventory);
        uiManager.CloseContentUI(UIType.ProfilePopup);
    }
}
