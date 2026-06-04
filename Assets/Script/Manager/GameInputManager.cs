using UnityEngine;


public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Inst { get; private set; }

    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsInteractDown { get; private set; }
    public bool IsHarvestDown { get; private set; }

    private readonly KeyCode[] _hotbarKeys = 
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
        KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
    };

    private void Awake() 
    {
        Inst = this; 
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        MoveInput = new Vector2(x, y).normalized;
        IsRunning = Input.GetKey(KeyCode.LeftShift);

        IsInteractDown = Input.GetKeyDown(KeyCode.F);
        IsHarvestDown = Input.GetKeyDown(KeyCode.Space);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UIManager.Inst.ToggleInventoryPopup();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Inst.CloseAllOpenedPopup();
        }

        CheckHotbarNumberInput();

        CheckHotbarScrollInput();
    }

    private void CheckHotbarNumberInput()
    {
        if (InventoryManager.Inst == null) return;

        for (int i = 0; i < _hotbarKeys.Length; i++)
        {
            if (Input.GetKeyDown(_hotbarKeys[i]))
            {
                InventoryManager.Inst.ChangeEquippedIndex(i);
                return; 
            }
        }
    }
    private void CheckHotbarScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll == 0f || InventoryManager.Inst == null) return;

        int currentIndex = InventoryManager.Inst.EquippedHotbarIndex;
        int max = InventoryManager.Inst.GetHotbarInventory().Length;

        if (scroll > 0f)
        {
            InventoryManager.Inst.ChangeEquippedIndex(currentIndex - 1 < 0 ? max - 1 : currentIndex - 1);
        }
        else
        {
            InventoryManager.Inst.ChangeEquippedIndex((currentIndex + 1) % max);
        }
    }
}