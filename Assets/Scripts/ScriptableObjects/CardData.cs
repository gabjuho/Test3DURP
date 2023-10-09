using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CardData", order = Int32.MaxValue, fileName = "CardData")]
public class CardData : ScriptableObject
{
    [SerializeField]
    private string id;

    public string ID => id;

    [SerializeField]
    private LeftCardData leftCardData;

    public LeftCardData LeftCardData => leftCardData;
    
    [SerializeField] 
    private RightCardData rightCardData;

    public RightCardData RightCardData => rightCardData;
    
    [ReadOnly, SerializeField]
    private int offeringCount;

    public int OfferingCount => offeringCount;
    
    [SerializeField]
    private Sprite cardImage;

    public Sprite CardImage => cardImage;

    [SerializeField] 
    private bool isOfferingCard;

    public bool IsOfferingCard => isOfferingCard;
    

    //오른쪽 카드 데이터와 왼쪽 카드 데이터를 합쳐 주는 함수
    public void SetCombiningCardData(LeftCardData leftCardData, RightCardData rightCardData)
    {
        this.leftCardData = leftCardData;
        this.rightCardData = rightCardData;
        offeringCount = leftCardData.OfferingCount + rightCardData.OfferingCount;
        cardImage = rightCardData.CardImage;
    }

    private void OnValidate()
    {
        offeringCount = leftCardData.OfferingCount + rightCardData.OfferingCount;
    }
}
