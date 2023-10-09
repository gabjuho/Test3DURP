using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardFieldHandler : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    private Image _cardFieldImage;
    private CardField _cardField;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _cardFieldImage = GetComponent<Image>();
        _cardField = GetComponent<CardField>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CardHandler.hasCard)
        {
            return;
        }

        CardHand.CurrentCardField = gameObject;

        _cardFieldImage.color = new Color32(255, 52, 52, 74);
        CardField.IsPlacingOnField = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!CardHandler.hasCard)
        {
            return;
        }
        
        _cardFieldImage.color = new Color32(255, 255, 255, 74);
        CardField.IsPlacingOnField = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!OfferingManager.Inst.IsOfferingMode)
        {
            return;
        }

        if (OfferingManager.Inst.IsEnoughOfferingCount && _cardField.IsEmptyField())
        {
            Vector3 canvasPosition = Utils.CardFieldAnchoredPositionToCanvasPosition(_rectTransform.anchoredPosition);
            
            OfferingManager.Inst.SummonCardOnMyCardField(canvasPosition, _rectTransform);
            return;
        }

        if (_cardField.IsOffering)
        {
            _cardField.IsOffering = false;
            OfferingManager.Inst.UnRegisterOfferingCard(gameObject);

        }
        else
        {
            _cardField.IsOffering = true;
            OfferingManager.Inst.RegisterOfferingCard(gameObject);
        }
    }
}
