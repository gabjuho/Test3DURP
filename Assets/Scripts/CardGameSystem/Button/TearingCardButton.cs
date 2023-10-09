using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TearingCardButton : MonoBehaviour
{
    [SerializeField]
    private GameObject leftCardPrefab;

    [SerializeField]
    private GameObject rightCardPrefab;
    
    private CardHand _cardHand;
    private GameObject _leftCardGroup;
    private GameObject _rightCardGroup;
    private RectTransform _rectTransform;

    private GameObject _leftCardObject;
    private GameObject _rightCardObject;

    private Card _card;
    private void Awake()
    {
        _cardHand = GameObject.Find("MyCardHand").GetComponent<CardHand>();
        _leftCardGroup = GameObject.Find("LeftCardGroup");
        _rightCardGroup= GameObject.Find("RightCardGroup");
        _rectTransform = GetComponent<RectTransform>();
        _card = GetComponent<Card>();
    }

    //TearingCardButton 클릭 시 해당 카드를 찢고 카드 보관함에 들어가게 하는 함수
    public void OnClickTearingCardButton()
    {
        if (!CombiningCardManager.Inst.HaveChanceToTearCard)
        {
            return;
        }
        _cardHand.RemoveCard(gameObject, true);
        
        _leftCardObject = Instantiate(leftCardPrefab, _rectTransform.position, Quaternion.identity, _leftCardGroup.transform);
        Card leftCard = _leftCardObject.GetComponent<Card>();
        RectTransform leftCardRectTransform = _leftCardObject.GetComponent<RectTransform>();
        leftCard.SetLeftCard(_card.CardData);
        
        _rightCardObject = Instantiate(rightCardPrefab, _rectTransform.position, Quaternion.identity, _rightCardGroup.transform);
        Card rightCard = _rightCardObject.GetComponent<Card>();
        RectTransform rightCardRectTransform = _rightCardObject.GetComponent<RectTransform>();
        rightCard.SetRightCard(_card.CardData);
        
        CombiningCardManager.Inst.AddTearingCardObject(_leftCardObject,_rightCardObject);
        
        leftCardRectTransform.DOAnchorPos(
                CombiningCardManager.Inst.ShowingTearingCardButtonRectTransform.anchoredPosition, 0.5f)
            .OnComplete(()=>
            {
                _leftCardObject.SetActive(false);
            });
        rightCardRectTransform.DOAnchorPos(
                CombiningCardManager.Inst.ShowingTearingCardButtonRectTransform.anchoredPosition, 0.5f)
            .SetDelay(0.1f)
            .OnComplete(() =>
            {
                _rightCardObject.SetActive(false);
            });

        Destroy(gameObject);
    }
}
