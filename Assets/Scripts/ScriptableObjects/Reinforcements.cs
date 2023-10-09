using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class Reinforcements : Ability
{
    private RectTransform _reinforceCardSpawnPoint;
    private GameObject _card;
    private GameObject _reinforcementsCardGroup;
    private CardFieldGroupController _cardFieldGroupController;
    public Reinforcements()
    {
        abilityImage = Resources.Load<Sprite>("Test/rainy");
        _card = Resources.Load<GameObject>("Prefabs/Card");
        
        _reinforceCardSpawnPoint = GameObject.Find("CardReinforcementsPoint").GetComponent<RectTransform>();
        _reinforcementsCardGroup = GameObject.Find("ReinforcementsCardGroup");
        _cardFieldGroupController = GameObject.Find("CardFieldGroup").GetComponent<CardFieldGroupController>();
    }

    public void UseMyAbility(CardField cardField, CardData cardData)
    {
        int cardFieldIndex = _cardFieldGroupController.GetMyCardFieldIndexByCardField(cardField);

        if (cardFieldIndex - 1 < 0)
        {
            return;
        }

        CardField summoningCardField = _cardFieldGroupController.GetMyCardFieldObjectByIndex(cardFieldIndex - 1).GetComponent<CardField>();
        RectTransform summoningCardFieldRectTransform = summoningCardField.gameObject.GetComponent<RectTransform>();
        Vector3 summoningCardFieldCanvasPosition = summoningCardFieldRectTransform.anchoredPosition;
        summoningCardFieldCanvasPosition = Utils.CardFieldAnchoredPositionToCanvasPosition(summoningCardFieldCanvasPosition);
        
        if (!summoningCardField.IsEmptyField())
        {
            return;
        }
        
        Debug.Log(cardFieldIndex);
        
        GameObject cardObject = GameObject.Instantiate(_card, _reinforceCardSpawnPoint.position, Quaternion.identity,
            _reinforcementsCardGroup.transform);

        Card card = cardObject.GetComponent<Card>();
        RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
        UnityEngine.UI.Image cardImage = card.gameObject.GetComponent<UnityEngine.UI.Image>();
        UnityEngine.UI.Image cardFieldImage = summoningCardField.gameObject.GetComponent<UnityEngine.UI.Image>();
        
        card.SetCard(cardData);
        cardImage.raycastTarget = false;
        cardFieldImage.raycastTarget = false;
        summoningCardField.PutGameObject(cardObject);

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(cardRectTransform.DOScale(new Vector2(1.3f, 1.3f), 0.2f))
            .Append(cardRectTransform.DOMove(summoningCardFieldCanvasPosition, 0.2f));
    }
    
    public void UseEnemyAbility(CardField cardField, CardData cardData)
    {
        int cardFieldIndex = _cardFieldGroupController.GetEnemyCardFieldIndexByCardField(cardField);

        if (cardFieldIndex - 1 < 0)
        {
            return;
        }

        CardField summoningCardField = _cardFieldGroupController.GetEnemyCardFieldObjectByIndex(cardFieldIndex - 1).GetComponent<CardField>();
        RectTransform summoningCardFieldRectTransform = summoningCardField.gameObject.GetComponent<RectTransform>();
        Vector3 summoningCardFieldCanvasPosition = summoningCardFieldRectTransform.anchoredPosition;
        summoningCardFieldCanvasPosition = Utils.CardFieldAnchoredPositionToCanvasPosition(summoningCardFieldCanvasPosition);
        
        if (!summoningCardField.IsEmptyField())
        {
            return;
        }
        
        Debug.Log(cardFieldIndex);
        
        GameObject cardObject = GameObject.Instantiate(_card, _reinforceCardSpawnPoint.position, Quaternion.identity,
            _reinforcementsCardGroup.transform);

        Card card = cardObject.GetComponent<Card>();
        RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
        UnityEngine.UI.Image cardImage = card.gameObject.GetComponent<UnityEngine.UI.Image>();
        UnityEngine.UI.Image cardFieldImage = summoningCardField.gameObject.GetComponent<UnityEngine.UI.Image>();
        
        card.SetCard(cardData);
        cardImage.raycastTarget = false;
        cardFieldImage.raycastTarget = false;
        summoningCardField.PutGameObject(cardObject);

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(cardRectTransform.DOSizeDelta(new Vector2(1.3f, 1.3f), 0.3f))
            .Append(cardRectTransform.DOMove(summoningCardFieldCanvasPosition, 0.3f));
    }
}
