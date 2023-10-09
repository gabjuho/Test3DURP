using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sacrifice : Ability
{
    private int sacrificingCount;

    public int SacrificingCount => sacrificingCount;

    public Sacrifice(int sacrificingCount)
    {
        this.sacrificingCount = sacrificingCount;
        abilityImage = Resources.Load<Sprite>("Test/human");
    }
}
