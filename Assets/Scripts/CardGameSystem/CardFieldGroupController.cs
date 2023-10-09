using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class CardFieldGroupController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] playerCardFieldObjects = new GameObject[CARD_FIELD_COUNT];

    [SerializeField]
    private GameObject[] enemyCardFieldObjects = new GameObject[CARD_FIELD_COUNT];

    private readonly Image[] _playerCardFieldImages = new Image[CARD_FIELD_COUNT];
    private readonly CardField[] _cardFields = new CardField[CARD_FIELD_COUNT];
    private readonly CardField[] _enemyCardFields = new CardField[CARD_FIELD_COUNT];

    public const int CARD_FIELD_COUNT = 4;

    private void Awake()
    {
        for (int i = 0; i < CARD_FIELD_COUNT; i++)
        {
            _playerCardFieldImages[i] = playerCardFieldObjects[i].GetComponent<Image>();
            _cardFields[i] = playerCardFieldObjects[i].GetComponent<CardField>();
            _enemyCardFields[i] = enemyCardFieldObjects[i].GetComponent<CardField>();
        }
    }

    //Me의 CardField 비활성화
    public void InActivateMyCardFields()
    {
        foreach (Image playerCardFieldImage in _playerCardFieldImages)
        {
            playerCardFieldImage.raycastTarget = false;
        }
    }
    
    //카드가 배치되어있는 필드도 강제로 raycastTarget을 True로 만든다.
    public void ActivateMyCardFieldsByForcing()
    {
        for (int i = 0; i < CARD_FIELD_COUNT; i++)
        {
            _playerCardFieldImages[i].raycastTarget = true;
        }
    }

    //카드를 가지고 있는 필드만 raycastTarget을 True로 만든다.
    public void ActivateMyCardFieldOnlyHavingCard()
    {
        for (int i = 0; i < CARD_FIELD_COUNT; i++)
        {
            if (!_cardFields[i].IsEmptyField())
            {
                _playerCardFieldImages[i].raycastTarget = true;
            }
            else
            {
                _playerCardFieldImages[i].raycastTarget = false;
            }
        }
    }
    
    //빈 카드 필드만 raycastTarget을 True로 만든다.
    public void ActivateMyEmptyCardField()
    {
        for (int i = 0; i < CARD_FIELD_COUNT; i++)
        {
            if (_cardFields[i].IsEmptyField())
            {
                _playerCardFieldImages[i].raycastTarget = true;
            }
            else
            {
                _playerCardFieldImages[i].raycastTarget = false;
            }
        }
    }

    //Enemy 필드에 있는 카드를 공격하는 함수
    public void AttackEnemy()
    {
        for (int i = 0; i < CARD_FIELD_COUNT; i++)
        {
            _cardFields[i].AttackCard();
        }
    }

    //Me 필드에 있는 카드를 공격하는 함수
    public void AttackMe()
    {
        for (int i = 0; i < CARD_FIELD_COUNT; i++)
        {
            _enemyCardFields[i].AttackCard();
        }
    }

    //특정 Enemy의 CardField가 비어있는 지 Index를 통해 체크할 수 있는 함수
    public bool IsEmptyEnemyCardFieldByIndex(int index)
    {
        if (index < 0 || index >= CARD_FIELD_COUNT)
        {
            Debug.LogError("인덱스 배열 범위 초과");
            return false;
        }
        return _enemyCardFields[index].IsEmptyField();
    }
    
    //return 값이 NULL일 수 있으면 해당 Attribute 사용을 권장
    [CanBeNull]
    //특정 My CardField Object을 index를 통해 가져올 수 있는 함수
    public GameObject GetMyCardFieldObjectByIndex(int index)
    {
        if (index < 0 || index >= CARD_FIELD_COUNT)
        {
            Debug.LogError("인덱스 배열 범위 초과");
            return null;
        }
        
        return playerCardFieldObjects[index];
    }
    
    [CanBeNull]
    //특정 Enemy의 CardField Object을 index를 통해 가져올 수 있는 함수
    public GameObject GetEnemyCardFieldObjectByIndex(int index)
    {
        if (index < 0 || index >= CARD_FIELD_COUNT)
        {
            Debug.LogError("인덱스 배열 범위 초과");
            return null;
        }
        
        return enemyCardFieldObjects[index];
    }


    //카드 필드들의 IsOffering 변수를 False로 변경해준다. (카드 필드의 Offering 모드 세팅을 초기화하기 위함)
    public void SetCardFieldUIIsOfferingToFalse()
    {
        for (int i = 0; i < CARD_FIELD_COUNT; i++)
        {
            _cardFields[i].IsOffering = false;
        }
    }

    //소환된 카드의 수를 반환하는 함수
    public int GetMySummonedCardCount()
    {
        int summonedCardCount = 0;
        for (int i = 0; i < CARD_FIELD_COUNT; i++)
        {
            if (!_cardFields[i].IsEmptyField())
            {
                summonedCardCount++;
            }
        }
        
        //자기 자신 제외
        summonedCardCount--;
        
        return summonedCardCount;
    }
    public int GetEnemySummonedCardCount()
    {
        int summonedCardCount = 0;
        for (int i = 0; i < CARD_FIELD_COUNT; i++)
        {
            if (!_enemyCardFields[i].IsEmptyField())
            {
                summonedCardCount++;
            }
        }

        //자기 자신 제외
        summonedCardCount--;
        
        return summonedCardCount;
    }

    //매개변수로 받아온 My CardField가 몇 번째 인덱스인지 반환해주는 함수
    public int GetMyCardFieldIndexByCardField(CardField cardField)
    {
        for (int i = 0; i < CARD_FIELD_COUNT; i++)
        {
            if (_cardFields[i] == cardField)
            {
                return i;
            }
        }

        return -1;
    }
    
    //매개변수로 받아온 Enemy CardField가 몇 번째 인덱스인지 반환해주는 함수
    public int GetEnemyCardFieldIndexByCardField(CardField cardField)
    {
        for (int i = 0; i < CARD_FIELD_COUNT; i++)
        {
            if (_enemyCardFields[i] == cardField)
            {
                return i;
            }
        }

        return -1;
    }
}
