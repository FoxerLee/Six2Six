using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    [SerializeField] private Canvas canvas;
    private GameObject draggedCard;

    public void OnBeginDrag(PointerEventData eventData) {
        draggedCard = new GameObject();
        draggedCard.transform.SetParent(canvas.transform);
        draggedCard.name = "draggedCard";
        draggedCard.transform.localScale = transform.localScale * 0.8f;

        Image image = draggedCard.AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>("tab-tmp");
        image.SetNativeSize();
        
        CanvasGroup canvasGroup = draggedCard.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        draggedCard.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        CanvasGroup canvasGroup = draggedCard.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (draggedCard != null) {
            Destroy(draggedCard);
        }
    }

}
