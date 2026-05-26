using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    [Header("프리팹 및 생성 영역")]
    [SerializeField] private GameObject Prefab_Slot; 
    [SerializeField] private Transform Transform_HotbarRoot; 

    [Header("툴바 설정")]
    [SerializeField] private int HotbarSlotCount = 10;

    private List<InventorySlotUI> _hotbarSlotList = new List<InventorySlotUI>();
    private int _currentSelectedIndex = 0; 

    private void Start()
    {
        InitHotbarSlots();

        SelectSlot(_currentSelectedIndex);
    }

    private void OnEnable()
    {
        RefreshHotbarSlots();
    }

    private void Update()
    {
        CheckAlphaNumericInput();
        CheckMouseScrollInput();

    }

    private void InitHotbarSlots()
    {
        if (Prefab_Slot == null || Transform_HotbarRoot == null) { return; }

        for (int i = 0; i < HotbarSlotCount; i++)
        {
            var gobj = Instantiate(Prefab_Slot, Transform_HotbarRoot);
            gobj.transform.localScale = Vector3.one; 
            gobj.name = $"HotbarSlot: {i}";

            var slotComponent = gobj.GetComponent<InventorySlotUI>();
            if (slotComponent != null)
            {
                _hotbarSlotList.Add(slotComponent);

                int index = i;
                slotComponent.InitSlot(index);

                slotComponent.BindSlotSelectEvent((id) => OnClick_HotbarSlot(index));
            }
        }
    }

    public void RefreshHotbarSlots()
    {
        if (InventoryManager.Inst == null) { return; }

        var hotbarDataArray = InventoryManager.Inst.GetHotbarInventory();
        if (hotbarDataArray == null) { return; }

        for (int i = 0; i < _hotbarSlotList.Count; i++)
        {
            var data = hotbarDataArray[i];

            _hotbarSlotList[i].UpdateSlot(data);
        }
    }

    private void CheckAlphaNumericInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6)) SelectSlot(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7)) SelectSlot(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8)) SelectSlot(7);
        else if (Input.GetKeyDown(KeyCode.Alpha9)) SelectSlot(8);
        else if (Input.GetKeyDown(KeyCode.Alpha0)) SelectSlot(9);

    }
    private void CheckMouseScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            int nextIndex = _currentSelectedIndex;

            if (scroll > 0f)
            {
                nextIndex--;
                if (nextIndex < 0) nextIndex = _hotbarSlotList.Count - 1;
            }
            else if (scroll < 0f)
            {
                nextIndex++;
                if (nextIndex >= _hotbarSlotList.Count) nextIndex = 0;
            }

            SelectSlot(nextIndex);
        }
    }

    private void OnClick_HotbarSlot(int index)
    {
        SelectSlot(index);
    }

    private void SelectSlot(int index)
    {
        if (index < 0 || index >= _hotbarSlotList.Count) return;

        _currentSelectedIndex = index;

        for (int i = 0; i < _hotbarSlotList.Count; i++)
        {
            _hotbarSlotList[i].ChangeSelectedState(i == _currentSelectedIndex);
        }

        Debug.Log($"[Hotbar] 현재 {index + 1}번 툴바 슬롯 선택됨!");
    }
}