using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    [SerializeField]
    private WinningGameState winningGameState;
    protected override void Die()
    {
        GameStateManager.Inst.ChangeGameState(winningGameState);
    }
}
