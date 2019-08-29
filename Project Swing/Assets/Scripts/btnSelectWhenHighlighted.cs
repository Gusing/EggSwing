using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class btnSelectWhenHighlighted : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current != null)
        {
            print("mouseover");
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
    }
}
