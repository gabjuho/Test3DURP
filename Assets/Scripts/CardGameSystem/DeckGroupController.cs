using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckGroupController : MonoBehaviour
{
    [SerializeField]
    private DeckController[] deckControllers = new DeckController[2];
    

    public enum EDeckOwner
    {
        Me,
        Enemy
    }
    
    [SerializeField]
    private EDeckOwner deckOwner;

    public EDeckOwner DeckOwner => deckOwner;

    //Me의 덱들을 전부 비활성화하는 함수
    public void InActivateAllDecks()
    {
        foreach (DeckController deckController in deckControllers)
        {
            deckController.InActivateDeck();
        }
    }
    
    //Me의 덱들을 전부 활성화하는 함수
    public void ActivateAllDecks()
    {
        if (deckOwner == EDeckOwner.Enemy)
        {
            return;
        }
        
        foreach (DeckController deckController in deckControllers)
        {
            deckController.ActivateDeck();
        }
    }
}
