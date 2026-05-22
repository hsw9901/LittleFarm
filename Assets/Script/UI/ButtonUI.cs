using UnityEngine;
using System;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class ButtonUI : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    
    public void BindOnClickButtonEvent(Action action)
    {
        if (_button == null) _button = GetComponent<Button>();

        _button.onClick.RemoveAllListeners(); 
        _button.onClick.AddListener(() => action?.Invoke());
    }
}
