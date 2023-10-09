using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnState : MonoBehaviour,GameState
{
    [SerializeField]
    private RectTransform playerTurnTextRectTransform;

    [SerializeField]
    private RectTransform playerTurnTextMovingRectTransform;

    [SerializeField]
    private EnemyTurnState enemyTurnState;

    [SerializeField]
    private LosingGameState losingGameState;

    [SerializeField]
    private CardFieldGroupController cardFieldGroupController;

    [SerializeField]
    private DeckGroupController deckGroupController;

    [SerializeField]
    private Button turningEndButton;

    private Vector3 playerTurnTextPosition;

    private void Awake()
    {
        playerTurnTextPosition = playerTurnTextRectTransform.anchoredPosition;
    }

    public void Execute()
    {
        SetPlayerTurn();
        ShowPlayerTurnText();
    }

    private void SetPlayerTurn()
    {
        ActivateDeck();
        ActivateCardField();
        CombiningCardManager.Inst.HaveChanceToTearCard = true;
        turningEndButton.interactable = true;
    }

    private void ShowPlayerTurnText()
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(playerTurnTextRectTransform.DOAnchorPos(playerTurnTextMovingRectTransform.position, 1f)
                .SetDelay(1f))
            .Append(playerTurnTextRectTransform.DOAnchorPos(playerTurnTextPosition, 1f)
                .SetDelay(1f));
    }

    //TurningEndButton을 클릭했을 때, State를 EnemyState로 변경하고 내 덱과 필드 비활성화 및 상대 공격을 실행하는 함수
    public void OnClickTurningEndButton()
    {
        InActivateCardField();
        InActivateDeck();
        AttackEnemyCard();
        GameStateManager.Inst.ChangeGameState(enemyTurnState);
    }

    private void ActivateDeck()
    {
        deckGroupController.ActivateAllDecks();
    }
    private void InActivateDeck()
    {
        deckGroupController.InActivateAllDecks();
    }

    private void ActivateCardField()
    {
        cardFieldGroupController.ActivateMyEmptyCardField();
    }
    private void InActivateCardField()
    {
        cardFieldGroupController.InActivateMyCardFields();
    }

    /*이부분은 CardFieldHandler에서 손보는 편이 좋을 수 있다. 아니면, CardFieldHandler에 있는 Attack함수를 해당 함수
    안에 캡슐화하는 방법도 있음*/
    private void AttackEnemyCard()
    {
        
    }
}
