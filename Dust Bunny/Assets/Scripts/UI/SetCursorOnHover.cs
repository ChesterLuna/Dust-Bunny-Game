using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SetCursorOnHover : MonoBehaviour
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(SelectedCursor.instance != null){
            Debug.Log("hehe");
            SelectedCursor.instance.SetTarget(gameObject);
        }   
    }
}
