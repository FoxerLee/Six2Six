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
        //GameObject draggedCard = eventData.pointerDrag.transform.Find("tab").gameObject;

        GameObject draggedCard = eventData.pointerDrag.GetComponent<Drag>().dropTab;
        Vector3 locationAdj = eventData.pointerDrag.GetComponent<Drag>().dropLoactionAdjustment;

        if (draggedCard != null) {    
            // Create a copy of the dragged card        
            droppedCard = Instantiate(draggedCard);
            droppedCard.transform.SetParent(transform);
            droppedCard.name = "power-up";
            droppedCard.transform.localScale = transform.localScale * 0.8f;
            droppedCard.transform.SetAsFirstSibling();

            droppedCard.transform.position = transform.position + locationAdj;
        }
    }

}
