using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour,IDragHandler,IPointerDownHandler,IPointerExitHandler,IPointerUpHandler,IPointerEnterHandler
{
    public static bool hasCard = false;
    
    private Vector3 _originPosition;
    private Quaternion _originRotation;
    private RectTransform _rectTransform;
    private int _originCardIndex;
    private Image _cardImage;

    private CardHand _myCardHand;
    private CardHand _enemyCardHand;
    private Card _card;

    [SerializeField]
    private GameObject tearingCardButtonObject;
    private readonly Vector3 _tearingCardButtonDefaultPoint = new Vector3(-540f, 40f, 0f);

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _cardImage = GetComponent<Image>();
        _myCardHand = GameObject.Find("MyCardHand").GetComponent<CardHand>();
        _enemyCardHand = GameObject.Find("EnemyCardHand").GetComponent<CardHand>();
        _card = GetComponent<Card>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_card.CardData.OfferingCount == 0)
        {
            RectTransform cardTransform = gameObject.GetComponent<RectTransform>();
            cardTransform.anchoredPosition = eventData.position;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _originCardIndex = _rectTransform.GetSiblingIndex();
        _rectTransform.SetAsLastSibling();
        
        if (_card.CardData.OfferingCount > 0 && !_card.IsCombined)
        {
            RectTransform tearingCardButtonRectTransform = tearingCardButtonObject.GetComponent<RectTransform>();
            tearingCardButtonRectTransform.rotation = Quaternion.identity;
        
            tearingCardButtonObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rectTransform.SetSiblingIndex(_originCardIndex);
        
        tearingCardButtonObject.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_card.CardData.OfferingCount == 0)
        {
            _cardImage.raycastTarget = true;
            hasCard = false;
            if (CardField.IsPlacingOnField)
            {
                RectTransform cardFieldRectTransform = CardHand.CurrentCardField.GetComponent<RectTransform>();
                //Canvas 기준 Anchored Position
                Vector3 cardFieldCanvasPosition = cardFieldRectTransform.anchoredPosition;
                cardFieldCanvasPosition = Utils.CardFieldAnchoredPositionToCanvasPosition(cardFieldCanvasPosition);
            
                _cardImage.raycastTarget = false;
                Image cardFieldImage = cardFieldRectTransform.gameObject.GetComponent<Image>();
                cardFieldImage.raycastTarget = false;
                cardFieldImage.color = new Color32(255, 255, 255, 74);
                CardField.IsPlacingOnField = false;

                PutCardOnMyField(cardFieldCanvasPosition, cardFieldRectTransform);
            }
            else
            {
                _rectTransform.DOMove(_originPosition, 0.3f);
                _rectTransform.DORotateQuaternion(_originRotation, 0.3f);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _originPosition = GetComponent<RectTransform>().anchoredPosition;
        _originRotation = GetComponent<RectTransform>().rotation;

        if (_card.CardData.OfferingCount == 0)
        {
            RectTransform cardTransform = gameObject.GetComponent<RectTransform>();

            cardTransform.anchoredPosition = eventData.position;
            _rectTransform.DORotateQuaternion(Quaternion.identity, 0.2f);
            _cardImage.raycastTarget = false;
            hasCard = true;
        }
        else
        {
            OfferingManager.Inst.EntryOfferingMode(gameObject);
        }
    }

    //카드를 Me의 CardField에 놓는 함수
    public void PutCardOnMyField(Vector3 cardFieldCanvasPosition, RectTransform cardFieldRectTransform)
    {
        _myCardHand.RemoveCard(gameObject, true);
        
        CardField cardField = cardFieldRectTransform.gameObject.GetComponent<CardField>();
        cardField.PutGameObject(gameObject);
        
        if (_card.CardData.LeftCardData.AbilityType == AbilityTable.EAbilityName.Link ||
            _card.CardData.RightCardData.AbilityType == AbilityTable.EAbilityName.Link)
        {
            GameObject cardFieldGroupObject = _card.CardField.transform.parent.gameObject;
            
            int increasingDamage = cardFieldGroupObject.GetComponent<CardFieldGroupController>().GetMySummonedCardCount();
            _card.IncreaseDamage(increasingDamage);
        }
        
        _rectTransform.DOMove(cardFieldCanvasPosition, 0.3f)
            .OnComplete(() =>
            {
                CardField enemyCardField = cardField.EnemyCardFieldObject.GetComponent<CardField>();
                if ((_card.CardData.LeftCardData.AbilityType == AbilityTable.EAbilityName.FirstHand ||
                     _card.CardData.RightCardData.AbilityType == AbilityTable.EAbilityName.FirstHand) &&
                    !enemyCardField.IsEmptyField())
                {
                    cardField.AttackCard();
                }
                
                if (_card.CardData.LeftCardData.AbilityType == AbilityTable.EAbilityName.Reinforcements ||
                    _card.CardData.RightCardData.AbilityType == AbilityTable.EAbilityName.Reinforcements)
                {
                    Debug.Log("증원");
                    Reinforcements reinforcements =
                        AbilityTable.AbilityDictionary[AbilityTable.EAbilityName.Reinforcements] as Reinforcements;
                    
                    reinforcements.UseMyAbility(cardField, _card.CardData);
                }
            });
        
 
        _rectTransform.SetAsFirstSibling();
    }

    //카드를 Enemy의 CardField에 놓는 함수
    public void PutCardOnEnemyField(Vector3 cardFieldCanvasPosition, RectTransform cardFieldRectTransform)
    {
        _enemyCardHand.RemoveCard(gameObject, false);
        
        CardField cardField = cardFieldRectTransform.gameObject.GetComponent<CardField>();
        cardField.PutGameObject(gameObject);
        
        if (_card.CardData.LeftCardData.AbilityType == AbilityTable.EAbilityName.Link ||
            _card.CardData.RightCardData.AbilityType == AbilityTable.EAbilityName.Link)
        {
            GameObject cardFieldGroupObject = _card.CardField.transform.parent.gameObject;
            
            int increasingDamage = cardFieldGroupObject.GetComponent<CardFieldGroupController>().GetEnemySummonedCardCount();
            _card.IncreaseDamage(increasingDamage);
        }

        _rectTransform.DOMove(cardFieldCanvasPosition, 0.5f)
            .OnComplete(() =>
            {
                CardField enemyCardField = cardField.EnemyCardFieldObject.GetComponent<CardField>();
                if ((_card.CardData.LeftCardData.AbilityType == AbilityTable.EAbilityName.FirstHand ||
                    _card.CardData.RightCardData.AbilityType == AbilityTable.EAbilityName.FirstHand) &&
                    !enemyCardField.IsEmptyField())
                {
                    cardField.AttackCard();
                }
                
                if (_card.CardData.LeftCardData.AbilityType == AbilityTable.EAbilityName.Reinforcements ||
                    _card.CardData.RightCardData.AbilityType == AbilityTable.EAbilityName.Reinforcements)
                {
                    Reinforcements reinforcements =
                        AbilityTable.AbilityDictionary[AbilityTable.EAbilityName.Reinforcements] as Reinforcements;
                    
                    reinforcements.UseEnemyAbility(cardField, _card.CardData);
                }
            });
        
        _rectTransform.SetAsFirstSibling();
    }
}
