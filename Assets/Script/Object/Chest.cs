using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("상자 설정")]
    public int ChestSize = 36;

    public ItemModel[] ChestInventory;

    private void Awake()
    {
        if (ChestInventory == null || ChestInventory.Length != ChestSize)
        {
            InitNewChest(ChestSize);
        }
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

    public void InitNewChest(int size)
    {
        ChestSize = size;

        ChestInventory = new ItemModel[size];

        for (int i = 0; i < size; i++)
        {
            ChestInventory[i] = new ItemModel { ItemDataId = "", ItemStackCount = 0 };
        }

        Debug.Log($"새 상자가 {size}칸으로 완벽하게 초기화되었습니다!");
    }
}