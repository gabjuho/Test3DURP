using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTable
{
    public enum EAbilityName
    {
        None,
        Sacrifice2,
        Sacrifice3,
        Breaking,
        DoubleSlash,
        Link,
        FirstHand,
        Reinforcements,
        Will
    }

    private static Dictionary<EAbilityName, Ability> _abilityDictionary = new Dictionary<EAbilityName, Ability>()
    {
        { EAbilityName.None, new None() },
        { EAbilityName.Sacrifice2, new Sacrifice(2) },
        { EAbilityName.Sacrifice3, new Sacrifice(3) },
        { EAbilityName.Breaking, new Breaking() },
        { EAbilityName.DoubleSlash, new DoubleSlash() },
        { EAbilityName.Link, new Link() },
        { EAbilityName.FirstHand , new FirstHand() },
        { EAbilityName.Reinforcements , new Reinforcements() }
    };

    public static Dictionary<EAbilityName, Ability> AbilityDictionary => _abilityDictionary;
}