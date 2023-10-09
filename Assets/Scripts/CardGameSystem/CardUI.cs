using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI cardNameText;

    [SerializeField]
    private TextMeshProUGUI damageText;

    [SerializeField]
    private TextMeshProUGUI healthText;

    [SerializeField]
    private Image cardImage;

    [SerializeField]
    private Sprite activatedOfferingSprite;

    [SerializeField]
    private Sprite inActivatedOfferingSprite;

    [SerializeField]
    private Image[] leftOfferingImages = new Image[2];

    [SerializeField]
    private Image[] rightOfferingImages = new Image[2];

    [SerializeField]
    private Image leftAbilityImage;

    [SerializeField]
    private Image rightAbilityImage;

    private Card card;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        card = GetComponent<Card>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    //CardData에 따른 CardUI 초기화하는 함수
    public void InitializeCardUI()
    {
        damageText.text = card.CardData.LeftCardData.Damage.ToString();
        healthText.text = card.CardData.RightCardData.Health.ToString();

        cardNameText.text = card.CardData.LeftCardData.LeftCardName + " " + card.CardData.RightCardData.RightCardName;

        cardImage.sprite = card.CardData.CardImage;

        leftAbilityImage.sprite = AbilityTable.AbilityDictionary[card.CardData.LeftCardData.AbilityType].AbilityImage;

        rightAbilityImage.sprite = AbilityTable.AbilityDictionary[card.CardData.RightCardData.AbilityType].AbilityImage;
        
        for (int i = 0; i < 2; i++)
        {
            if (!card.CardData.LeftCardData.IsActivatedOffering[i])
            {
                leftOfferingImages[i].color = new Color32(0, 0, 0, 255);
            }

            if (!card.CardData.RightCardData.IsActivatedOffering[i])
            {
                rightOfferingImages[i].color = new Color32(0, 0, 0, 255);
            }
        }
    }

    //LeftCardData에 따른 왼쪽 CardUI 초기화하는 함수
    public void InitializeLeftCardUI()
    {
        damageText.text = card.CardData.LeftCardData.Damage.ToString();
        cardNameText.text = card.CardData.LeftCardData.LeftCardName;
        cardImage.sprite = card.CardData.LeftCardData.CardImage;

        leftAbilityImage.sprite = AbilityTable.AbilityDictionary[card.CardData.LeftCardData.AbilityType].AbilityImage;

        for (int i = 0; i < 2; i++)
        {
            if (!card.CardData.LeftCardData.IsActivatedOffering[i])
            {
                leftOfferingImages[i].color = new Color32(0, 0, 0, 255);
            }
        }
    }
    
    //RightCardData에 따른 오른쪽 CardUI 초기화하는 함수
    public void InitializeRightCardUI()
    {
        healthText.text = card.CardData.RightCardData.Health.ToString();
        cardNameText.text = card.CardData.RightCardData.RightCardName;
        cardImage.sprite = card.CardData.RightCardData.CardImage;

        rightAbilityImage.sprite = AbilityTable.AbilityDictionary[card.CardData.RightCardData.AbilityType].AbilityImage;

        for (int i = 0; i < 2; i++)
        {
            if (!card.CardData.RightCardData.IsActivatedOffering[i])
            {
                rightOfferingImages[i].color = new Color32(0, 0, 0, 255);
            }
        }
    }

    //CardUI 갱신하는 함수
    public void UpdateCardUI()
    {
        damageText.text = card.Damage.ToString();
        healthText.text = card.Health.ToString();
    }

    //카드를 FadeOut하는 함수
    public void FadeOut()
    {
        canvasGroup.DOFade(0f, 0.6f);
    }
}