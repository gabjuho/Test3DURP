using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RightCardData
{
    [SerializeField]
    private int health;

    public int Health => health;
    
    [SerializeField]
    private string rightCardName;

    public string RightCardName => rightCardName;

    [SerializeField, Range(0,2)]
    private int offeringCount;

    public int OfferingCount => offeringCount;

    [SerializeField]
    private bool[] isActivatedOffering = new bool[2];

    public bool[] IsActivatedOffering => isActivatedOffering;

    [SerializeField]
    private AbilityTable.EAbilityName abilityType;

    public AbilityTable.EAbilityName AbilityType => abilityType;
    
    [SerializeField]
    private Sprite cardImage;

    public Sprite CardImage => cardImage;
}
