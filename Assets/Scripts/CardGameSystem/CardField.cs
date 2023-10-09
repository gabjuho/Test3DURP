using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

//필드에 올라와 있는 카드와 필드의 상태를 관리하는 클래스
public class CardField : MonoBehaviour
{
    //카드를 필드 위에 올려놓았는 지 체크해주는 변수
    public static bool IsPlacingOnField = false;

    //해당 불 변수는 CardField에서 안전하게 변경할 수 있도록 함수 제공해주기

    [SerializeField] private GameObject cardOnField = null;
    public GameObject CardOnField => cardOnField;

    [SerializeField] private GameObject enemyCardFieldObject;
    public GameObject EnemyCardFieldObject => enemyCardFieldObject;

    [SerializeField] private Player enemy;

    //공격 횟수
    private int _attackCount = 0;

    public bool IsOffering { get; set; } = false;

    //해당 필드에 카드가 있는 지 없는 지 체크해주는 함수
    public bool IsEmptyField()
    {
        if (cardOnField == null)
        {
            return true;
        }

        return false;
    }

    //카드를 CardField에 놓는 함수
    public void PutGameObject(GameObject cardObject)
    {
        cardOnField = cardObject;
        Card card = cardObject.GetComponent<Card>();
        card.SetCardField(this);
    }

    //CardField에 있는 카드를 삭제하는 함수 (데이터만 삭제)
    public void RemoveCard()
    {
        cardOnField = null;
    }

    //반대편 필드에 있는 카드를 공격하는 함수
    public void AttackCard()
    {
        if (IsEmptyField())
        {
            return;
        }

        RectTransform cardOnFieldRectTransform = cardOnField.GetComponent<RectTransform>();
        RectTransform enemyCardFieldObjectRectTransform = enemyCardFieldObject.GetComponent<RectTransform>();
        CardField enemyCardField = enemyCardFieldObject.GetComponent<CardField>();
        Card card = cardOnField.GetComponent<Card>();

        Sequence sequence = DOTween.Sequence();

        Vector3 cardFieldCanvasPosition = enemyCardFieldObjectRectTransform.anchoredPosition;
        cardFieldCanvasPosition.x += 300f;
        cardFieldCanvasPosition.y = 880f + cardFieldCanvasPosition.y;

        _attackCount++;

        if (enemyCardField.IsEmptyField())
        {
            enemy.TakeDamage(card.Damage);
        }
        else
        {
            Card enemyCard = enemyCardField.CardOnField.GetComponent<Card>();
            bool isDead = enemyCard.BeDamagedCard(card.Damage);

            if ((card.CardData.LeftCardData.AbilityType == AbilityTable.EAbilityName.Breaking ||
                 card.CardData.RightCardData.AbilityType == AbilityTable.EAbilityName.Breaking) &&
                isDead == true)
            {
                enemy.TakeDamage(card.Damage);
            }
        }
        
        sequence
            .Append(cardOnFieldRectTransform.DOAnchorPos(cardFieldCanvasPosition, 0.2f))
            .Append(cardOnFieldRectTransform.DORotate(new Vector3(0f, 0f, 30f), 0.1f))
            .SetLoops(2, LoopType.Yoyo)
            .SetRecyclable(true)
            .OnComplete(() =>
            {
                if ((card.CardData.LeftCardData.AbilityType == AbilityTable.EAbilityName.DoubleSlash ||
                     card.CardData.RightCardData.AbilityType == AbilityTable.EAbilityName.DoubleSlash)
                    && _attackCount < 2)
                {
                    AttackCard();
                }
                else if ((card.CardData.LeftCardData.AbilityType == AbilityTable.EAbilityName.DoubleSlash ||
                          card.CardData.RightCardData.AbilityType == AbilityTable.EAbilityName.DoubleSlash) &&
                         _attackCount >= 2)
                {
                    _attackCount = 0;
                }
            });
    }
}