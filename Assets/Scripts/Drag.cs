﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [SerializeField] private Canvas canvas;
    public GameObject dropTab;
    public Vector3 dropLoactionAdjustment;
    private GameObject draggedCard;

    public void OnBeginDrag(PointerEventData eventData) {
        // Get the game object of the dragged card

        // Cteate a copy of the dragged card
        draggedCard = new GameObject();
        draggedCard.transform.SetParent(canvas.transform);
        draggedCard.name = "draggedCard";
        draggedCard.transform.localScale = transform.localScale * 0.8f;

        Image image = draggedCard.AddComponent<Image>();
        image.sprite = dropTab.GetComponent<Image>().sprite;
        image.SetNativeSize();
        
        // Set alpha
        CanvasGroup canvasGroup = draggedCard.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        draggedCard.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        // Restore alpha
        CanvasGroup canvasGroup = draggedCard.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Destroy
        if (draggedCard != null) {
            Destroy(draggedCard);
        }
    }

}