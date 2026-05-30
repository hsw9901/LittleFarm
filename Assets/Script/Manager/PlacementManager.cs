using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks; 

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Inst { get; private set; }

    [Header("설정")]
    public LayerMask GroundLayer;
    private GameObject _previewObject;
    private ItemData _currentItemData;
    private GameObject _loadedPrefab;

    private void Awake()
    {
        Inst = this;
    }

    private void Update()
    {
        CheckEquippedItem();

        if (_previewObject != null && _previewObject.activeSelf)
        {
            UpdatePreviewPosition();

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceObject();
            }
        }
    }

    private void CheckEquippedItem()
    {
        if (InventoryManager.Inst == null || GameDataManager.Inst == null) return;

        int equippedIndex = InventoryManager.Inst.EquippedHotbarIndex;
        ItemModel equippedItem = InventoryManager.Inst.GetHotbarItem(equippedIndex);

        if (equippedItem == null || string.IsNullOrEmpty(equippedItem.ItemDataId) || equippedItem.ItemStackCount <= 0)
        {
            ClearPreview();
            return;
        }

        if (GameDataManager.Inst.ItemDataList.TryGetValue(equippedItem.ItemDataId, out ItemData data))
        {
            if (data.GetItemUseType() == ItemUseType.Interactable && !string.IsNullOrEmpty(data.PrefabKey))
            {
                if (_currentItemData != data)
                {
                    _currentItemData = data;
                    LoadPreviewAsync(data).Forget();
                }
            }
            else
            {
                ClearPreview();
            }
        }
    }

    private async UniTaskVoid LoadPreviewAsync(ItemData data)
    {
        ClearPreview();


        _loadedPrefab = await ResourceManager.Inst.LoadAsset<GameObject>(data.PrefabKey);

        if (_currentItemData != data || _loadedPrefab == null) { return; }
        
        _previewObject = Instantiate(_loadedPrefab);
        DisableComponentsForPreview(_previewObject);
    }

    private void UpdatePreviewPosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        if (!_previewObject.activeSelf)
        {
            _previewObject.SetActive(true);
        }

        //_previewObject.transform.position = mouseWorldPos;
        _previewObject.transform.position = new Vector3(Mathf.Round(mouseWorldPos.x), Mathf.Round(mouseWorldPos.y), 0);
    }

    private void PlaceObject()
    {
        if (_previewObject == null || !_previewObject.activeSelf || _loadedPrefab == null) return;

        GameObject newChestObj = Instantiate(_loadedPrefab, _previewObject.transform.position, Quaternion.identity);

        Chest newChest = newChestObj.GetComponent<Chest>();
        if (newChest != null)
        {
            newChest.InitNewChest(36);
        }

        InventoryManager.Inst.ConsumeHotbarItem(InventoryManager.Inst.EquippedHotbarIndex);
    }

    private void ClearPreview()
    {
        if (_previewObject != null)
        {
            Destroy(_previewObject);
            _previewObject = null;
        }
        _loadedPrefab = null;
    }

    private void DisableComponentsForPreview(GameObject obj)
    {
        var colliders = obj.GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders) col.enabled = false;

        var chestScript = obj.GetComponent<Chest>();
        if (chestScript != null) chestScript.enabled = false;
    }
}