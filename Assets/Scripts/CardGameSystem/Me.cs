using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Me : Player
{
    [SerializeField]
    private LosingGameState losingGameState;
    protected override void Die()
    {
        GameStateManager.Inst.ChangeGameState(losingGameState);
    }
}
