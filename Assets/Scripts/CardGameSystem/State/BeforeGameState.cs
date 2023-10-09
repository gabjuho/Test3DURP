using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class BeforeGameState : MonoBehaviour, GameState
{
    [SerializeField]
    private GameObject suggestingCardPanel;
    [SerializeField]
    private GameObject suggestingCardGroup;

    [SerializeField]
    private GameObject suggestingCardPrefab;

    [SerializeField]
    private DeckController deckController;
    
    // enemyDeckController 추가
    [SerializeField]
    private DeckController enemyDeckController;

    [SerializeField]
    private List<RectTransform> suggestingCardPoint;

    [SerializeField]
    private RectTransform suggestingCardSpawnPoint;

    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private PlayerTurnState playerTurnState;

    [SerializeField]
    private CardHand myCardHand;

    private List<GameObject> suggestingCardObjects;
    
    private const int SUGGESTED_CARD_COUNT = 2;

    private void Awake()
    {
        suggestingCardObjects = new List<GameObject>();
    }

    public void Execute()
    {
        for (int i = 0; i < SUGGESTED_CARD_COUNT; i++)
        {
            enemyDeckController.DrawCardFromDeck(true);
        }
        
        ShuffleDeckRandomly();
        ShowSuggestingCards();
    }

    private void ShuffleDeckRandomly()
    {
        deckController.ShuffleDeckDataRandomly();
    }

    private void ShowSuggestingCards()
    {
        suggestingCardGroup.SetActive(true);

        for (int i = 0; i < SUGGESTED_CARD_COUNT; i++)
        {
            GameObject tempCardObject = Instantiate(suggestingCardPrefab, suggestingCardPoint[i].transform);
            suggestingCardObjects.Add(tempCardObject);

            //GetComponent 대신 TryGetComponent가 안전함 수정 요망
            Card card = tempCardObject.GetComponent<Card>();
            CardData firstCardDataInDeck = deckController.PopCardDataInDeckData();
            card.SetCard(firstCardDataInDeck);
        }
    }
    
    //제안된 카드를 손에 있는 카드로 옮기고 제안 모드를 비활성화하는 함수
    public void ChangeHandCard()
    {
        GameObject suggestingFirstCard = suggestingCardObjects.First();
        GameObject suggestingLastCard = suggestingCardObjects.Last();
        
        BeDisableSuggestingCard();
        
        RemoveAndRefillSuggestingCard(suggestingFirstCard);
        RemoveAndRefillSuggestingCard(suggestingLastCard);

        FadeOutSuggestingPanelImage();
        StartCoroutine(ChangeSuggestingCardToHandCard());
    }

    private IEnumerator ChangeSuggestingCardToHandCard()
    {
        yield return new WaitForSeconds(2.8f);
        
        foreach (GameObject cardObject in suggestingCardObjects)
        {
            Card card = cardObject.GetComponent<Card>();
            RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
            myCardHand.AddCard(true, cardPrefab, card.CardData, cardRectTransform.position);
        }

        while (suggestingCardObjects.Count != 0)
        {
            GameObject firstCardObject = suggestingCardObjects.First();
            Destroy(firstCardObject);
            suggestingCardObjects.RemoveAt(0);
        }
        GameStateManager.Inst.ChangeGameState(playerTurnState);
    }

    private void BeDisableSuggestingCard()
    {
        foreach (GameObject suggestingCard in suggestingCardObjects)
        {
            Image suggestingCardImage = suggestingCard.GetComponent<Image>();
            suggestingCardImage.raycastTarget = false;
        }
    }

    private void FadeOutSuggestingPanelImage()
    {
        Image suggestingCardPanelImage = suggestingCardPanel.GetComponent<Image>();
        suggestingCardPanelImage.DOFade(0f, 0.8f)
            .SetDelay(2.5f)
            .OnComplete(() =>
            {
                suggestingCardPanelImage.gameObject.SetActive(false);
            });
    }

    //좋지 않은 함수 네이밍과 함수 기능 구조는 아니다. (2개의 기능이 한번에 들어가 있고, And를 쓰는 네이밍은 좋지 않음)
    private void RemoveAndRefillSuggestingCard(GameObject suggestingCard)
    {
        SelectingCardUI selectingCardUI = suggestingCard.GetComponent<SelectingCardUI>();
        RectTransform selectingCardPointRectTransform = suggestingCard.transform.parent.GetComponent<RectTransform>();
        
        if (selectingCardUI.WillRemove)
        {
            AddCardDataInDeck(suggestingCard);
            RemoveSuggestingCard(suggestingCard, selectingCardUI);
            RefillSuggestingCard(selectingCardPointRectTransform);
            ShuffleDeckRandomly();
        }
    }

    private void AddCardDataInDeck(GameObject selectingCard)
    {
        Card card = selectingCard.GetComponent<Card>();
        deckController.AddCardDataInLast(card.CardData);
    }
    

    private void RemoveSuggestingCard(GameObject suggestingCard, SelectingCardUI selectingFirstCardUI)
    {
        suggestingCardObjects.Remove(suggestingCard);
        selectingFirstCardUI.RemoveSelectingCardSelf();
    }

    private void RefillSuggestingCard(RectTransform selectingCardPointRectTransform)
    {
        GameObject tempCardObject = Instantiate(suggestingCardPrefab, 
            suggestingCardSpawnPoint.anchoredPosition,
            Quaternion.identity
            );
        
        BeDisableSuggestingCard();
        
        RectTransform tempCardRectTransform = tempCardObject.GetComponent<RectTransform>();
        suggestingCardObjects.Add(tempCardObject);
        
        Card tempCard = tempCardObject.GetComponent<Card>();
        CardData tempCardData = deckController.PopCardDataInDeckData();
        tempCard.SetCard(tempCardData);
        tempCardObject.transform.SetParent(selectingCardPointRectTransform.transform);
        
        tempCardObject.SetActive(false);

        tempCardRectTransform.DOAnchorPos(Vector2.zero, 0.5f)
            .SetDelay(1f)
            .OnPlay(() =>
            {
                tempCardObject.SetActive(true);
            });
    }
}
