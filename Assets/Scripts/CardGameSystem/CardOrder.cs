using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class CardOrder : MonoBehaviour
{
    private int _originOrder;
    private RectTransform _rectTransform;
    
    // [SerializeField] Renderer[] middleRenderers;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    
    public void SetOriginOrder(int order)
    {
        _originOrder = order;
        SetOrder(_originOrder);
    }

    public void SetOrder(int order)
    {
        Vector3 cardPos = _rectTransform.position;
        cardPos.z = order;
        
        _rectTransform.position = cardPos;
    }
}