using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using DG.Tweening;
using UnityEngine.UIElements;

public class Card : MonoBehaviour
{
    private bool _isFront;
    private RectTransform _rectTransform;
    public PRS originPRS;

    private int health;
    public int Health => health;
    
    private int damage;
    public int Damage => damage;

    [SerializeField]
    private int offeringCount;
    public int OfferingCount => offeringCount;
    
    [SerializeField]
    private CardData cardData;
    public CardData CardData => cardData;
    
    private CardUI cardUI;
    
    private CardField cardField;
    public CardField CardField => cardField;

    public bool IsCombined { get; set; } = false;

    private void Awake()
    {
        cardUI = GetComponent<CardUI>();
        _rectTransform = GetComponent<RectTransform>();
    }

    //카드의 데이터와 UI를 세팅하는 함수
    public void SetCard(CardData cardData)
    {
        this.cardData = cardData;
        damage = cardData.LeftCardData.Damage;
        health = cardData.RightCardData.Health;
        offeringCount = cardData.OfferingCount;
        
        cardUI.InitializeCardUI();
    }

    //카드의 데이터와 왼쪽 UI를 세팅하는 함수
    public void SetLeftCard(CardData cardData)
    {
        this.cardData = cardData;
        damage = cardData.LeftCardData.Damage;
        health = cardData.RightCardData.Health;
        offeringCount = cardData.OfferingCount;
        
        cardUI.InitializeLeftCardUI();
    }
    //카드의 데이터와 오른쪽 UI를 세팅하는 함수
    public void SetRightCard(CardData cardData)
    {
        this.cardData = cardData;
        damage = cardData.LeftCardData.Damage;
        health = cardData.RightCardData.Health;
        offeringCount = cardData.OfferingCount;
        
        cardUI.InitializeRightCardUI();
    }

    //카드 앞면 뒷면을 세팅하는 함수
    public void Setup(bool isFront)
    {
        this._isFront = isFront;

        if (this._isFront)
        {
            // 카드 앞면 이미지
        }
        else
        {
            // 카드 뒷면 이미지
        }
    }

    //Transform 값들을 특정 Transform 값으로 옮겨주는 Util 함수 (useDotween = true -> Dotween 사용, false -> 즉시 이동)
    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            _rectTransform.DOMove(prs.pos, dotweenTime);
            _rectTransform.DORotateQuaternion(prs.rot, dotweenTime);
            _rectTransform.DOScale(prs.scale, dotweenTime);
        }
        else
        {
            {
                _rectTransform.position = prs.pos;
                _rectTransform.rotation = prs.rot;
                _rectTransform.localScale = prs.scale;
            }
        }
    }

    //상대 카드에게 Damage를 주는 함수 (반환값 true: 죽음, false: 생존)
    public bool BeDamagedCard(int damage)
    {
        bool isDead = false;
        
        health -= damage;
        
        if (health <= 0)
        {
            health = 0;
            KillCard();
            isDead = true;
        }
        cardUI.UpdateCardUI();
        
        return isDead;
    }

    //카드를 죽이는 함수
    public void KillCard()
    {
        _rectTransform.DOShakeRotation(0.7f, new Vector3(0f, 0f, 30f))
            .SetAutoKill(true)
            .OnStart(cardUI.FadeOut)
            .OnComplete(() =>
            {
                cardField.RemoveCard();
            })
            .OnKill(() =>
            {
                Destroy(gameObject);
            });
    }

    //Card가 위치해있는 CardField를 세팅해주는 함수
    public void SetCardField(CardField cardField)
    {
        this.cardField = cardField;
    }

    public void IncreaseDamage(int damage)
    {
        this.damage += damage;
        
        cardUI.UpdateCardUI();
    }
}

