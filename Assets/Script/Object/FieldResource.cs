using UnityEngine;

public class FieldResource : MonoBehaviour
{
    [Header("채집 설정")]
    public ToolCategory RequiredTool; 
    public int MaxHP = 3;             
    private int _currentHP;

    [Header("드랍 설정")]
    public string DropItemDataId;     
    public int DropMinCount = 1;      
    public int DropMaxCount = 3;      

    private void Awake()
    {
        _currentHP = MaxHP;
    }
    private void Start()
    {
        if (GameObjectManager.Inst != null)
            GameObjectManager.Inst.RegisterResource(this); 
    }

    private void OnDestroy()
    {
        if (GameObjectManager.Inst != null)
            GameObjectManager.Inst.UnregisterResource(this);
    }

    private void BreakResource()
    {
        int dropAmount = Random.Range(DropMinCount, DropMaxCount + 1);

        if (GameObjectManager.Inst != null && dropAmount > 0)
        {
            GameObjectManager.Inst.SpawnDropItem(DropItemDataId, dropAmount, transform.position);
        }

        Destroy(gameObject);
    }

    public void TakeHit(ToolCategory hitTool, int damage)
    {
        if (RequiredTool != ToolCategory.None && hitTool != RequiredTool)
        {
            Debug.Log($"알맞은 도구가 아님 ({RequiredTool} 필요)");
            return;
        }

        _currentHP -= damage;


        if (_currentHP <= 0)
        {
            BreakResource();
        }
    }
}