using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIInputReciever : InputReciever
{
    [SerializeField] private UnityEvent ClickEvent;
    public override void OnInputRecieved()
    {
        foreach (var handler in inputHandlers)
        {
            handler.ProcessInput(Input.mousePosition, gameObject, () => ClickEvent.Invoke());
        }
    }

}