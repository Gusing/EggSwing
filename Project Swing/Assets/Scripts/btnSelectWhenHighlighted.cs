using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class btnSelectWhenHighlighted : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        print("mouseover");
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
