using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Inst { get; private set; }

    [Header("UI 패널 연결")]
    [SerializeField] private GameObject InventoryPanel; 

    private void Awake()
    {
        Inst = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (InventoryPanel != null)
        {
            bool isActive = InventoryPanel.activeSelf;

            InventoryPanel.SetActive(!isActive);
        }
    }
}