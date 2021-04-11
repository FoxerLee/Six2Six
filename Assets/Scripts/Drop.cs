using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag != null) {
            Debug.Log("ON DROP");
        }
    }

}
