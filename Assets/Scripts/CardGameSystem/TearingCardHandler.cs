using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TearingCardHandler : MonoBehaviour,IPointerClickHandler
{
    private Image cardImage;
    public enum ECardType
    {
        Left,
        Right
    }

    public ECardType cardType;

    private void Awake()
    {
        cardImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardType == ECardType.Left && !CombiningCardManager.Inst.IsPlacedLeftCard)
        {
            CombiningCardManager.Inst.PlaceOnLeftCardFrame(gameObject);
        }
        else if(cardType == ECardType.Right && !CombiningCardManager.Inst.IsPlacedRightCard)
        {
            CombiningCardManager.Inst.PlaceOnRightCardFrame(gameObject);
        }

        cardImage.raycastTarget = false;
    }
}
