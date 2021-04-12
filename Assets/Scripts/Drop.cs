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

        // Get the game object of the dragged card
        GameObject draggedCard = eventData.pointerDrag.transform.Find("tab").gameObject;

        if (draggedCard != null) {    
            // Create a copy of the dragged card        
            droppedCard = new GameObject();
            droppedCard.transform.SetParent(transform);
            droppedCard.name = "power-up";
            droppedCard.transform.localScale = transform.localScale * 0.8f;
            droppedCard.transform.SetAsFirstSibling();

            Image image = droppedCard.AddComponent<Image>();
            image.sprite = draggedCard.GetComponent<Image>().sprite;
            image.SetNativeSize();

            droppedCard.transform.position = transform.position + new Vector3(0, 35, 0);
        }
    }

}
