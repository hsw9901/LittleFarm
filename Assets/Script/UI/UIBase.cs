using UnityEngine;

public abstract class FarmUIBase : MonoBehaviour
{
    protected virtual void Awake() { }
    public virtual void OnOpen() { }
    public virtual void OnClose() { }
}

