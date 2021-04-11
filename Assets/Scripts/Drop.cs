using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{

    private GameObject droppedCard;

    public void OnDrop(PointerEventData eventData) {
        // Check if the button already has a power-up card
        if (transform.Find("power-up") != null) {
            Debug.Log("Already has a power-up card!");
            return;
        }

        if (eventData.pointerDrag != null) {            
            droppedCard = new GameObject();
            droppedCard.transform.SetParent(transform);
            droppedCard.name = "power-up";
            droppedCard.transform.localScale = transform.localScale * 0.8f;
            droppedCard.transform.SetAsFirstSibling();

            Image image = droppedCard.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>("tab-tmp");
            image.SetNativeSize();

            droppedCard.transform.position = transform.position + new Vector3(0, 35, 0);
        }
    }

}
