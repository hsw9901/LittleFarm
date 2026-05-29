using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("상자 설정")]
    public int ChestSize = 36;

    public ItemModel[] ChestInventory;

    private void Awake()
    {
        ChestInventory = new ItemModel[ChestSize];
    }

    public void OpenChest()
    {
        Debug.Log("상자 열기");
        if (UIManager.Inst != null)
        {
            UIManager.Inst.OpenChestPopup(this);
        }
    }

    public bool TryPutItem(ItemModel item)
    {
        for (int i = 0; i < ChestSize; i++) 
        {
            if (ChestInventory[i] == null || string.IsNullOrEmpty(ChestInventory[i].ItemDataId))
            {
                ChestInventory[i] = item;
                return true;
            }
        }
        return false;
    }

}
