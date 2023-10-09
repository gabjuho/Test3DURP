using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTurnState : MonoBehaviour,GameState
{
    public void Execute()
    {
        CombiningCardManager.Inst.HaveChanceToTearCard = false;
        EnemyAI.Inst.Operate();
    }

}
