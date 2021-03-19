using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TapZone : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent method;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        method?.Invoke();
    }
}
