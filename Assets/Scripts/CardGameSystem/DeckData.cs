using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeckData
{
    [SerializeField]
    private List<CardData> cardDataInDeck;
    public List<CardData> CardDataInDeck
    {
        get => cardDataInDeck;
        set => cardDataInDeck = value;
    }
}
