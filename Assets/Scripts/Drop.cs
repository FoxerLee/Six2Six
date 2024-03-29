﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{

    [SerializeField] private GameObject gameController;


    public static Vector3 DropLocationAdjustment = new Vector3(0, 64, 0);

    private GameObject droppedCard;

    public void OnDrop(PointerEventData eventData)
    {
        // Check if the button already has a power-up card
        if (transform.Find("power-up") != null)
        {
            // Debug.Log("Already has a power-up card!");
            return;
        }

        // Get the game object of the dragged card
        GameObject tab = eventData.pointerDrag.GetComponent<Drag>().dropTab;

        if (tab != null)
        {
            // Create a copy of the dragged card        
            droppedCard = Instantiate(tab);
            droppedCard.transform.SetParent(transform);
            droppedCard.name = "power-up";
            droppedCard.transform.localScale = transform.localScale * 0.8f;
            droppedCard.transform.SetAsFirstSibling();
            droppedCard.transform.position = transform.position + Drop.DropLocationAdjustment;
        }

        // Update attachedEffects dictionary
        Game game = gameController.GetComponent<Game>();
        if (game.isRedTurn)
        {
            game.redAttachedEffects.Add(int.Parse(name), tab);
        }
        else
        {
            game.grayAttachedEffects.Add(int.Parse(name), tab);
        }

        // Decrease the number of times that the player can use a power-up
        if (game.isRedTurn)
        {
            game.redNumOfPowerUps--;
        }
        else
        {
            game.grayNumOfPowerUps--;
        }

        // Destroy dragged card
        eventData.pointerDrag.GetComponent<Drag>().DestroyDraggedCard();

        // Destroy the used power-up card in card deck
        Destroy(eventData.pointerDrag);

        // Disable all power-ups cards in deck
        game.DisablePowerUps();

        // Sound effect
        game.playSound(Game.SoundOptions.powerUp);
    }

}