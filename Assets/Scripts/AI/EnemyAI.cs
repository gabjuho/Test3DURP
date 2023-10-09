using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using DG.Tweening;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Inst { get; private set; }
    
    private BehaviorTreeRunner _BTRunner = null;

    private int _offeringCondition;
    private bool _isAnimationRunning = false;
    
    enum EPhase
    {   
        None = 0,
        DrawPhase,
        SummonPhase,
        AttackPhase,
    }

    private EPhase _phase = EPhase.None;
    
    // 카드 데이터 변수 접근 관련된 변수 목록. 일단 단계적으로 사용해봤음. 추후 수정
    
    [FormerlySerializedAs("_enemy")] [SerializeField]
    private Enemy enemy;
    
    [FormerlySerializedAs("_player")] [SerializeField]
    private Player player;
    
    
    [SerializeField]
    private PlayerTurnState playerTurnState;
    
    [SerializeField]
    private GameObject cardFieldGroup;
    [SerializeField]
    private CardFieldGroupController cardFieldGroupController;

    [SerializeField]
    private CardField[] playerFields = new CardField[CardFieldGroupController.CARD_FIELD_COUNT];
    [SerializeField]
    private CardField[] enemyFields = new CardField[CardFieldGroupController.CARD_FIELD_COUNT];
    
    [SerializeField]
    private Card[] playerFieldCards = new Card[CardFieldGroupController.CARD_FIELD_COUNT];
    [SerializeField]
    private Card[] enemyFieldCards = new Card[CardFieldGroupController.CARD_FIELD_COUNT];
    
    [SerializeField]
    private DeckController customDeckController;
    [SerializeField]
    private DeckController offeringDeckController;
    [SerializeField]
    private CardHand enemyCardHand;
    [SerializeField]
    private GameObject enemyCardHandGameObject;

    [SerializeField]
    private List<Card> enemyHandCards;
    private Card _selectedCard;
    
    void Awake()
    {
        Inst = this;
        cardFieldGroupController = cardFieldGroup.GetComponent<CardFieldGroupController>();

        _BTRunner = new BehaviorTreeRunner(SettingBT());
    }

    void Update()
    {
        if (!_isAnimationRunning)
        {
            if (_phase == EPhase.DrawPhase || _phase == EPhase.SummonPhase)
            {
                _BTRunner.Operate();
            }
            else if (_phase == EPhase.AttackPhase)
            {
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    public void Operate()
    {
        SituationUpdate();
        _phase = EPhase.DrawPhase;
    }

    public void SituationUpdate()
    {
        for (int i = 0; i < CardFieldGroupController.CARD_FIELD_COUNT; i++)
        {
            // 임시로 FieldGroup의 자식 Component 값을 불러오게 했지만 버그 발생 위험도가 크기 때문에 이후에 수정 필요 
            
            if (cardFieldGroup.transform.GetChild(i + 4) != null)
            {
                playerFields[i] = cardFieldGroup.transform.GetChild(i + 4).GetComponent<CardField>();
                if (playerFields[i].CardOnField != null)
                {
                    playerFieldCards[i] = playerFields[i].CardOnField.GetComponent<Card>();
                }
            }
            
            if (cardFieldGroup.transform.GetChild(i) != null)
            {
                enemyFields[i] = cardFieldGroup.transform.GetChild(i).GetComponent<CardField>();
                if (enemyFields[i].CardOnField != null)
                {
                    enemyFieldCards[i] = enemyFields[i].CardOnField.GetComponent<Card>();
                }
            }
        }

        enemyHandCards = enemyCardHand.HandCards.ToList();
    }

    INode SettingBT()
    {
        return new SequenceNode // 전체 시퀀스
        (
            new List<INode>()
            {
                new SequenceNode // 드로우 시퀀스
                (
                    new List<INode>()
                    {
                        new SelectorNode
                        (
                            new List<INode>()
                            {
                                // new ActionNode() 사망,
                                new ActionNode(ActionCheckAIDead),
                                // new ActionNode() 소환수 드로우,
                                new ActionNode(ActionDrawCustomDeck),
                                // new ActionNode() 제물 드로우
                                new ActionNode(ActionDrawOfferingDeck)
                            }
                        )
                    }
                ),
                new SequenceNode // 전개 시퀀스
                (
                    new List<INode>()
                    {
                        new SelectorNode // Selector 1
                        (
                            new List<INode>()
                            {
                                new SelectorNode // Selector 2
                                (
                                    new List<INode>()
                                    {
                                        // new ActionNode() 킬각 턴 종료,
                                        new ActionNode(ActionCanKillPlayer),
                                        new SelectorNode
                                        (
                                            new List<INode>()
                                            {     
                                                // new ActionNode() 킬각 라인 소환,
                                                new ActionNode(ActionSummonCardToKillPlayer),
                                                // new ActionNode() 소환수 정면 소환(발악),
                                                new ActionNode(ActionSummonToSurvive),
                                                // new ActionNode() 잡을 수 있는 카드 정면 소환, 
                                                new ActionNode(ActionSummonToKillCard), 
                                                // new ActionNode() 잡을 수 없는 카드 정면 소환(우세),
                                                new ActionNode(ActionSummonInBetterField),
                                                // new ActionNode() 잡을 수 없는 카드 정면 소환(불리),
                                                new ActionNode(ActionSummonInWorseField),
                                                // new ActionNode() 상대 에이스 카드 정면 소환, 
                                                new ActionNode(ActionSummonToAttackBestCard),
                                                // new ActionNode() 코스트가 제일 낮은 카드 소환
                                                new ActionNode(ActionSummonLowestCard)
                                            }             
                                        )
                                    }
                                )
                            }
                        )
                    }
                )
            }
        );
    }

    private int CheckOfferingCount()
    {
        return CheckOfferingCountInHand() + CheckOfferingCountInField();
    }
    
    private int CheckOfferingCountInHand()
    {
        int offeringCount = 0;

        foreach (Card card in enemyHandCards)
            if (card.OfferingCount <= _offeringCondition)
                offeringCount++;

        return offeringCount;
    }
    
    private int CheckOfferingCountInField()
    {
        int offeringCount = 0;

        for (int i = 0; i < CardFieldGroupController.CARD_FIELD_COUNT; i++)
        {
            if (enemyFieldCards[i] != null)
            {
                if (enemyFieldCards[i].OfferingCount <= _offeringCondition)
                {
                    offeringCount++;
                }
            }
        }

        return offeringCount;
    }

    private bool CheckOfferingEnough(int cost)
    {
        if (CheckOfferingCount() >= cost)
        {
            return true;
        }

        return false;
    }
    
    public GameObject SelectCardFieldObject(int fieldIndex)
    {
        if (cardFieldGroupController.IsEmptyEnemyCardFieldByIndex(fieldIndex))
        {
            return cardFieldGroupController.GetEnemyCardFieldObjectByIndex(fieldIndex);
        }

        return null;
    }

    void KillOfferings(int cost, int fieldIndex = -1)
    {
        int minCost = 99;
        int lowestCardIndex = 0;
        List<int> killedIndex = new List<int>();

        if (fieldIndex != -1)
        {
            killedIndex.Add(fieldIndex);
            enemyFieldCards[fieldIndex].KillCard();
            cost -= 1;
        }

        for (int i = 0; i < cost; i++)
        {
            for (int j = 0; j < CardFieldGroupController.CARD_FIELD_COUNT; j++)
            {
                if (!enemyFields[j].IsEmptyField() && !killedIndex.Contains(j))
                {
                    if (minCost > enemyFieldCards[j].OfferingCount);
                    {
                        minCost = enemyFieldCards[j].OfferingCount;
                        lowestCardIndex = j;
                    }
                }
            }
            
            Debug.Log(lowestCardIndex);
            if (!enemyFields[lowestCardIndex].IsEmptyField())
            {
                minCost = 99;
                killedIndex.Add(lowestCardIndex);
                enemyFieldCards[lowestCardIndex].KillCard();
            }
        }
    }

    IEnumerator CustomDeckDraw()
    {
        _isAnimationRunning = true;
        customDeckController.DrawCardFromDeck(true);

        yield return new WaitForSeconds(1.0f);
        _isAnimationRunning = false;
        SituationUpdate();
        _phase = EPhase.SummonPhase;
    }

    IEnumerator OfferingDeckDraw()
    {
        _isAnimationRunning = true;
        offeringDeckController.DrawCardFromDeck(true);

        yield return new WaitForSeconds(1.0f);
        _isAnimationRunning = false;
        SituationUpdate();
        _phase = EPhase.SummonPhase;
        
    }

    IEnumerator SummonInField(int fieldIndex)
    {
        _isAnimationRunning = true;
        if (_selectedCard != null)
        {
            yield return new WaitForSeconds(1.0f);
            
            CardHandler firstCard = _selectedCard.GetComponent<CardHandler>();
            Image firstCardImage = firstCard.GetComponent<Image>();
            firstCardImage.raycastTarget = false;

            GameObject cardFieldObject = SelectCardFieldObject(fieldIndex);
            RectTransform cardFieldRectTransform = cardFieldObject.GetComponent<RectTransform>();
            Vector3 cardFieldCanvasPosition = cardFieldRectTransform.anchoredPosition;
            cardFieldCanvasPosition.x += 300f;
            cardFieldCanvasPosition.y = 880f + cardFieldCanvasPosition.y;

            if (cardFieldObject != null)
            {
                firstCard.PutCardOnEnemyField(cardFieldCanvasPosition, cardFieldRectTransform);
            }
        }
        
        yield return new WaitForSeconds(1.0f);
        _isAnimationRunning = false;
        _selectedCard = null;
        SituationUpdate();
    }
    
    IEnumerator AttackCoroutine()
    {
        _isAnimationRunning = true;
        cardFieldGroupController.AttackMe();

        yield return new WaitForSeconds(1.0f);
        
        _selectedCard = null;
        _offeringCondition = 0;
        _isAnimationRunning = false;
        _phase = EPhase.None;

        GameStateManager.Inst.ChangeGameState(playerTurnState);
    }
    
    // new ActionNode() 사망,
    INode.ENodeState ActionCheckAIDead()
    {
        if (enemy.Health <= 0)
        {
            return INode.ENodeState.Success;
            Debug.Log("죽어있는디?");
        }
        return INode.ENodeState.Failure;
    }
    // new ActionNode() 소환수 드로우,
    INode.ENodeState ActionDrawCustomDeck()
    {
        if (_phase == EPhase.DrawPhase)
        {
            if (!customDeckController.IsEmptyDeckData())
            {
                int minCost = 99;

                if (enemyHandCards.Any())
                {
                    foreach (Card card in enemyHandCards)
                    {
                        if (card.OfferingCount < minCost && card.OfferingCount > 0)
                        {
                            minCost = card.OfferingCount;
                        }
                    }
                
                    if (CheckOfferingCount() >= minCost)
                    {
                        Debug.Log("소환수 드로우");
                        StartCoroutine(CustomDeckDraw());
                        return INode.ENodeState.Success;
                    }
                }
                else
                {
                    Debug.Log("패에 카드가 없음");
                    StartCoroutine(CustomDeckDraw());
                    return INode.ENodeState.Success;
                }
            }
        }
        
        return INode.ENodeState.Failure;
    }
    // new ActionNode() 제물 드로우
    INode.ENodeState ActionDrawOfferingDeck()
    {
        if (_phase == EPhase.DrawPhase)
        {
            Debug.Log("제물 드로우");
            StartCoroutine(OfferingDeckDraw());
            return INode.ENodeState.Success;  
        }
        
        return INode.ENodeState.Success;
    }
    
    // new ActionNode() 킬각 턴 종료,
    INode.ENodeState ActionCanKillPlayer()
    {
        int enemyFieldTotalDamage = 0;
        
        for (int i = 0; i < CardFieldGroupController.CARD_FIELD_COUNT; i++)
        {
            if (!enemyFields[i].IsEmptyField())
            {
                enemyFieldTotalDamage += enemyFieldCards[i].Damage;
            }
        }

        if (enemyFieldTotalDamage >= player.Health)
        {
            Debug.Log("킬각 턴 종료");
            _phase = EPhase.AttackPhase;
            return INode.ENodeState.Success;
        }

        return INode.ENodeState.Failure;
    }
    
    // new ActionNode() 킬각 라인 소환, 
    INode.ENodeState ActionSummonCardToKillPlayer()
    {
        Debug.Log("A");
        for (int i = 0; i < CardFieldGroupController.CARD_FIELD_COUNT; i++)
        {
            if (playerFields[i].IsEmptyField() && enemyFields[i].IsEmptyField())
            {
                foreach (Card card in enemyHandCards)
                {
                    if (card.Damage >= player.Health && CheckOfferingEnough(card.OfferingCount))
                    {
                        _offeringCondition = 99;
                        if (CheckOfferingCountInField() >= card.OfferingCount)
                        {
                            Debug.Log("킬각 라인 소환");
                            
                            KillOfferings(_selectedCard.OfferingCount);
                            StartCoroutine(SummonInField(i));
                            _phase = EPhase.AttackPhase;
                            return INode.ENodeState.Success;
                        }
                    }
                }
            }
        }

        return INode.ENodeState.Failure;
    }
    // new ActionNode() 소환수 정면 소환(발악), 
    INode.ENodeState ActionSummonToSurvive()
    {
        Debug.Log("B");
        int totalDamage = 0;
        int maxDamageCardIndex = 0;

        for (int i = 0; i < CardFieldGroupController.CARD_FIELD_COUNT; i++)
        {
            if (enemyFields[i].IsEmptyField() && !playerFields[i].IsEmptyField())
            {
                totalDamage += playerFieldCards[i].Damage;
                if (maxDamageCardIndex > playerFieldCards[i].Damage)
                {
                    maxDamageCardIndex = playerFieldCards[i].Damage;
                }
            }
        }

        if (totalDamage >= enemy.Health)
        {
            foreach (Card card in enemyHandCards)
            {
                if (card.OfferingCount == 0)
                {
                    _selectedCard = card;
                }
            }

            if (_selectedCard != null && CheckOfferingCountInField() >= _selectedCard.OfferingCount)
            {
                Debug.Log("소환수 정면 소환(발악)");
                KillOfferings(_selectedCard.OfferingCount);
                StartCoroutine(SummonInField(maxDamageCardIndex));
                return INode.ENodeState.Success;
            }
        }

        return INode.ENodeState.Failure;
    }
    
    // new ActionNode() 잡을 수 있는 카드 정면 소환, 
    INode.ENodeState ActionSummonToKillCard()
    {
        Debug.Log("C");
        for (int i = 0; i < CardFieldGroupController.CARD_FIELD_COUNT; i++)
        {
            if (!playerFields[i].IsEmptyField())
            {
                foreach (Card card in enemyHandCards)
                {
                    if (playerFieldCards[i].Health <= card.Damage && CheckOfferingCountInField() >= card.OfferingCount)
                    {
                        _selectedCard = card;
                        Debug.Log("잡을 수 있는 카드 정면 소환");
                    
                        if (enemyFields[i].IsEmptyField())
                        {
                            KillOfferings(_selectedCard.OfferingCount);
                            StartCoroutine(SummonInField(i));
                            return INode.ENodeState.Success;
                        }
                        else if (enemyFieldCards[i].OfferingCount == _offeringCondition && _selectedCard.OfferingCount > 0)
                        {
                            KillOfferings(_selectedCard.OfferingCount, i);
                            StartCoroutine(SummonInField(i));
                            return INode.ENodeState.Success;
                        }
                    }
                }
            }
        }
        return INode.ENodeState.Failure;
    }
    
    // new ActionNode() 잡을 수 없는 카드 정면 소환(우세), 
    INode.ENodeState ActionSummonInBetterField()
    {
        Debug.Log("D");

        for (int i = 0; i < CardFieldGroupController.CARD_FIELD_COUNT; i++)
        {
            if (!playerFields[i].IsEmptyField())
            {
                foreach (Card card in enemyHandCards)
                {
                    if (playerFieldCards[i].Health * playerFieldCards[i].Damage >= card.Health * card.Damage
                        && CheckOfferingCountInField() >= card.OfferingCount)
                    {
                        _selectedCard = card;

                        Debug.Log("소환수 정면 소환(우세)");
                        if (enemyFields[i].IsEmptyField())
                        {
                            KillOfferings(_selectedCard.OfferingCount);
                            StartCoroutine(SummonInField(i));
                            return INode.ENodeState.Success;
                        }
                        else if (enemyFieldCards[i].OfferingCount == _offeringCondition &&
                                 _selectedCard.OfferingCount > 0)
                        {
                            KillOfferings(_selectedCard.OfferingCount, i);
                            StartCoroutine(SummonInField(i));
                            return INode.ENodeState.Success;
                        }
                    }
                }
            }
        }
        
        return INode.ENodeState.Failure;
    }
    
    // new ActionNode() 잡을 수 없는 카드 정면 소환(불리),
    INode.ENodeState ActionSummonInWorseField()
    {
        Debug.Log("E");

        for (int i = 0; i < CardFieldGroupController.CARD_FIELD_COUNT; i++)
        {
            if (!playerFields[i].IsEmptyField())
            {
                foreach (Card card in enemyHandCards)
                {
                    if (playerFieldCards[i].Damage <= card.Health && CheckOfferingCountInField() >= card.OfferingCount)
                    {
                        _selectedCard = card;

                        Debug.Log("소환수 정면 소환(불리)");
                        if (enemyFields[i].IsEmptyField())
                        {
                            KillOfferings(_selectedCard.OfferingCount);
                            StartCoroutine(SummonInField(i));
                            return INode.ENodeState.Success;
                        }
                        else if (enemyFieldCards[i].OfferingCount == _offeringCondition &&
                                 _selectedCard.OfferingCount > 0)
                        {
                            KillOfferings(_selectedCard.OfferingCount, i);
                            StartCoroutine(SummonInField(i));
                            return INode.ENodeState.Success;
                        }
                    }
                }
            }
        }
        
        return INode.ENodeState.Failure;
    }
    
    // new ActionNode() 상대 에이스 카드 정면 소환,
    INode.ENodeState ActionSummonToAttackBestCard()
    {
        Debug.Log("F");

        for (int i = 0; i < CardFieldGroupController.CARD_FIELD_COUNT; i++)
        {
            if (!playerFields[i].IsEmptyField())
            {
                foreach (Card card in enemyHandCards)
                {
                    if (playerFieldCards[i].OfferingCount >= 3 && playerFieldCards[i].Health <= card.Damage * 2
                        && CheckOfferingCountInField() >= card.OfferingCount)
                    {
                        _selectedCard = card;

                        Debug.Log("에이스 카드 공격");
                        if (enemyFields[i].IsEmptyField())
                        {
                            KillOfferings(_selectedCard.OfferingCount);
                            StartCoroutine(SummonInField(i));
                            return INode.ENodeState.Success;
                        }
                        else if (enemyFieldCards[i].OfferingCount == _offeringCondition &&
                                 _selectedCard.OfferingCount > 0)
                        {
                            KillOfferings(_selectedCard.OfferingCount, i);
                            StartCoroutine(SummonInField(i));
                            return INode.ENodeState.Success;
                        }
                    }
                }
            }
        }
        
        return INode.ENodeState.Failure;
    }

    // new ActionNode() 코스트가 제일 낮은 카드 소환
    INode.ENodeState ActionSummonLowestCard()
    {
        Debug.Log("H");
        int minCost = 99;
        foreach (Card card in enemyHandCards)
        {
            if (card.OfferingCount < minCost && CheckOfferingEnough(card.OfferingCount))
            {
                minCost = card.OfferingCount;
                _selectedCard = card;
            }
        }
        
        for (int i = 0; i < CardFieldGroupController.CARD_FIELD_COUNT; i++)
        {
            if (_selectedCard != null)
            {
                Debug.Log("최저 코스트 소환");                    
                if (enemyFields[i].IsEmptyField())
                {
                    KillOfferings(_selectedCard.OfferingCount);
                    StartCoroutine(SummonInField(i));
                    return INode.ENodeState.Success;
                }
                else if (enemyFieldCards[i].OfferingCount == _offeringCondition && _selectedCard.OfferingCount > 0)
                {
                    KillOfferings(_selectedCard.OfferingCount, i);
                    StartCoroutine(SummonInField(i));
                    return INode.ENodeState.Success;
                }
            }
        }

        if (_phase == EPhase.SummonPhase)
        {
            Debug.Log("go attack phase");
            _phase = EPhase.AttackPhase;
        }

        return INode.ENodeState.Failure;
    }
}
