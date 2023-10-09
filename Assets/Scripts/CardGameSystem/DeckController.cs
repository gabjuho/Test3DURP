using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = System.Random;

public class DeckController : MonoBehaviour
{
    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField] 
    private DeckData deckData;

    [SerializeField]
    private DeckGroupController deckGroupController;

    [SerializeField]
    private CardHand cardHand;

    private Image deckImage;

    private void Awake()
    {
        deckImage = GetComponent<Image>();
    }
    //덱으로부터 카드를 Draw하는 함수
    public void DrawCardFromDeck(bool isOwnerEnemy)
    {
        if (IsEmptyDeckData())
        {
            return;
        }
        
        CardData firstCardData = PopCardDataInDeckData();

        if (!isOwnerEnemy)
        {
            cardHand.AddCardFromDeck(true, cardPrefab, firstCardData);
        }
        else
        {
            cardHand.AddCardFromDeck(false, cardPrefab, firstCardData);
        }
    }

    //덱의 CardData 중 첫번째 CardData를 가져오는 함수
    public CardData GetFirstCardDataInDeckData()
    {
        if (IsEmptyDeckData())
        {
            Debug.LogError("DeckData is empty");
            return null;
        }
        return deckData.CardDataInDeck.First();
    }

    //덱의 CardData 중 첫번째 CardData를 삭제하는 함수
    public void RemoveFirstCardDataInDeckData()
    {
        if (IsEmptyDeckData())
        {
            Debug.LogError("DeckData is empty");
        }
        deckData.CardDataInDeck.RemoveAt(0);
    }

    //덱의 CardData 마지막에 새 CardData를 추가하는 함수
    public void AddCardDataInLast(CardData cardData)
    {
        deckData.CardDataInDeck.Add(cardData);
    }

    //첫번째 CardData를 DeckData에서 꺼낸 후 삭제하는 함수
    public CardData PopCardDataInDeckData()
    {
        CardData resultCardData = GetFirstCardDataInDeckData();
        RemoveFirstCardDataInDeckData();
        return resultCardData;
    }
    
    //덱이 비어있는 지 체크하는 함수
    public bool IsEmptyDeckData()
    {
        return deckData.CardDataInDeck.Count == 0;
    }

    //랜덤하게 덱을 셔플하는 함수
    public void ShuffleDeckDataRandomly()
    {
        deckData.CardDataInDeck = Utils.ShuffleDeckData(deckData.CardDataInDeck);
    }

    //덱의 UI를 활성화하는 함수
    public void ActivateDeck()
    {
        deckImage.raycastTarget = true;
    }

    //덱의 ui를 비활성화하는 함수
    public void InActivateDeck()
    {
        deckImage.raycastTarget = false;
    }

}
