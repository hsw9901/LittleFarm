using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    protected virtual void Awake() { }
    public virtual void OnOpen() { }
    public virtual void OnClose() { }
}

