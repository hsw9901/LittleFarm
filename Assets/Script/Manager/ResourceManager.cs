using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class ResourceManager : MonoBehaviour
{
   public static ResourceManager Inst {  get; private set; }

    private Dictionary<string, AsyncOperationHandle> _handles = new();

    private void Awake()
    {
        Inst = this;
    }

    public async UniTask<T> LoadAsset<T>(string address) where T : UnityEngine.Object
    {
        if (_handles.TryGetValue(address, out var handle))
            return handle.Result as T;

        var loadHandle = Addressables.LoadAssetAsync<T>(address);
        try
        {
            var result = await loadHandle.ToUniTask();
            _handles[address] = loadHandle;
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FarmResourceManager] 로드 실패: {address} / {e.Message}");
            if (loadHandle.IsValid()) Addressables.Release(loadHandle);
            return null;
        }
    }

    public void Release(string address)
    {
        if (_handles.TryGetValue(address, out var handle))
        {
            Addressables.Release(handle);
            _handles.Remove(address);
        }
    }

}
