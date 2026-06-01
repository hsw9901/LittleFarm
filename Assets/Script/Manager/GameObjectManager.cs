using UnityEngine;
using System.Collections.Generic;

public class GameObjectManager : MonoBehaviour
{
    public static GameObjectManager Inst { get; private set; }

    [Header("드랍 아이템 설정")]
    public GameObject Prefab_DropItem; 

    [Header("현재 맵에 있는 오브젝트들")]
    public List<Chest> PlacedChests = new List<Chest>();
    public List<FieldResource> FieldResources = new List<FieldResource>();

    private void Awake()
    {
        Inst = this;
    }

    public void RegisterChest(Chest chest)
    {
        if (!PlacedChests.Contains(chest)) PlacedChests.Add(chest);
    }

    public void UnregisterChest(Chest chest)
    {
        if (PlacedChests.Contains(chest)) PlacedChests.Add(chest);
    }

    public void RegisterResource(FieldResource resource)
    {
        if (!FieldResources.Contains(resource)) FieldResources.Add(resource);
    }

    public void UnregisterResource(FieldResource resource)
    {
        if (FieldResources.Contains(resource)) FieldResources.Remove(resource);
    }

    public void SpawnDropItem(string itemDataId, int count, Vector3 position)
    {
        
        if (Prefab_DropItem == null)
        {
            InventoryManager.Inst.AddItem(itemDataId, count);
            Debug.Log($"[매니저] {itemDataId} {count}개를 인벤토리로 바로 지급합니다.");
            return;
        }

        
        Debug.Log($"[매니저] {itemDataId} 아이템이 바닥에 {count}개 떨어졌습니다!");
    }
}