using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = System.Numerics.Quaternion;

public class CombiningCardManager : MonoBehaviour
{
    public static CombiningCardManager Inst { get; private set; }

    [SerializeField]
    private GameObject combiningCardPanel;
    
    [SerializeField]
    private RectTransform showingTearingCardButtonRectTransform;

    [SerializeField]
    private RectTransform leftCardSpawnPointRectTransform;
    [SerializeField]
    private RectTransform leftCardEndPointRectTransform;
    [SerializeField]
    private RectTransform rightCardSpawnPointRectTransform;
    [SerializeField]
    private RectTransform rightCardEndPointRectTransform;

    [SerializeField]
    private RectTransform leftCardFrameRectTransform;

    [SerializeField]
    private RectTransform rightCardFrameRectTransform;

    [SerializeField]
    private RectTransform cardFrameRectTransform;

    [SerializeField]
    private CardHand cardHand;

    public RectTransform ShowingTearingCardButtonRectTransform => showingTearingCardButtonRectTransform;
    
    private List<GameObject> leftCards = new List<GameObject>();
    private List<GameObject> rightCards = new List<GameObject>();
    
    public bool HaveChanceToTearCard { get; set; } = true;

    private bool _isPlacedLeftCard = false;
    public bool IsPlacedLeftCard => _isPlacedLeftCard;
    
    private bool _isPlacedRightCard = false;
    public bool IsPlacedRightCard => _isPlacedRightCard;

    private int combiningCardCount = 0;

    private GameObject _leftCard;
    private GameObject _rightCard;

    [SerializeField]
    private GameObject cardPrefab;

    void Awake()
    {
        Inst = this;
    }

    //카드 합성 모드를 진입하는 함수
    public void EntryCombiningCardMode()
    {
        combiningCardPanel.SetActive(true);

        for (int i = 0; i < leftCards.Count; i++)
        {
            leftCards[i].transform.SetParent(combiningCardPanel.transform);
            RectTransform leftCardRectTransform = leftCards[i].GetComponent<RectTransform>();
            leftCardRectTransform.anchoredPosition = leftCardSpawnPointRectTransform.anchoredPosition;
            leftCards[i].SetActive(true);
            
            rightCards[i].transform.SetParent(combiningCardPanel.transform);
            RectTransform rightCardRectTransform = rightCards[i].GetComponent<RectTransform>();
            rightCardRectTransform.anchoredPosition = rightCardSpawnPointRectTransform.anchoredPosition;
            rightCards[i].SetActive(true);
        }
        ActivateTearingCards();

        OrderCards();
    }
    
    private void ActivateTearingCards()
    {
        for (int i = 0; i < leftCards.Count; i++)
        {
            Image leftCardImage = leftCards[i].GetComponent<Image>();

            leftCardImage.raycastTarget = true;
        }
        
        for (int i = 0; i < rightCards.Count; i++)
        {
            Image rightCardImage = rightCards[i].GetComponent<Image>();

            rightCardImage.raycastTarget = true;
        }
    }
    
    private void InActivateTearingCards()
    {
        for (int i = 0; i < leftCards.Count; i++)
        {
            Image leftCardImage = leftCards[i].GetComponent<Image>();

            leftCardImage.raycastTarget = false;
        }
        
        for (int i = 0; i < rightCards.Count; i++)
        {
            Image rightCardImage = rightCards[i].GetComponent<Image>();

            rightCardImage.raycastTarget = false;
        }
    }
    
    private void OrderCards()
    {
        List<Vector3> leftCardPositions = new List<Vector3>();
        List<Vector3> rightCardPositions = new List<Vector3>();
        
        // 내 패 정렬
        leftCardPositions = MakeCardPositions(leftCards, leftCardSpawnPointRectTransform, leftCardEndPointRectTransform);
        rightCardPositions = MakeCardPositions(rightCards, rightCardSpawnPointRectTransform, rightCardEndPointRectTransform);
        
        for (int i = 0; i < leftCards.Count; i++)
        {
            RectTransform leftCardRectTransform = leftCards[i].GetComponent<RectTransform>();
            leftCardRectTransform.DOAnchorPos(leftCardPositions[i], 0.5f).SetDelay(0.1f);
        }
        for (int i = 0; i < rightCards.Count; i++)
        {
            RectTransform rightCardRectTransform = rightCards[i].GetComponent<RectTransform>();
            rightCardRectTransform.DOAnchorPos(rightCardPositions[i], 0.5f).SetDelay(0.1f);
        }
    }

    private List<Vector3> MakeCardPositions(List<GameObject> cards, RectTransform cardSpawnPointRectTransform, RectTransform cardEndPointRectTransform)
    {
        int objCount = cards.Count;
        float[] objLerps = new float[objCount];
        List<Vector3> results = new List<Vector3>(objCount);

        switch (objCount)
        {
            case 1:  // 패에 카드가 1장일 때
                objLerps = new float[] { 0.5f };
                break;
            case 2:  //  '' 2장일 때
                objLerps = new float[] { 0.27f, 0.73f };
                break; 
            case 3:  //  '' 3장일 때
                objLerps = new float[] { 0.1f, 0.5f, 0.9f };
                break;
            default: // 4장일 때 부터 회전값 추가
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        for (int i = 0; i < objCount; i++)
        {
            Vector3 targetPos = Vector3.Lerp(
                cardSpawnPointRectTransform.anchoredPosition, cardEndPointRectTransform.anchoredPosition, objLerps[i]);
            results.Add(targetPos);
        }

        return results;
    }

    //CombiningCardMode를 나가는 함수
    public void ExitCombiningCardMode()
    {
        InActivateTearingCards();
        combiningCardPanel.SetActive(false);
    }
    
    //찢겨진 카드를 찢겨진 카드 컨테이너에 추가하는 함수
    public void AddTearingCardObject(GameObject leftCardObject, GameObject rightCardObject)
    {
        leftCards.Add(leftCardObject);
        rightCards.Add(rightCardObject);
        HaveChanceToTearCard = false;
    }

    //LeftCardFrame에 LeftCard를 놓는 함수
    public void PlaceOnLeftCardFrame(GameObject leftCardObject)
    {
        _leftCard = leftCardObject;
        _isPlacedLeftCard = true;
        combiningCardCount++;
        
        RectTransform leftCardRectTransform = leftCardObject.GetComponent<RectTransform>();
        leftCardRectTransform.DOAnchorPos(leftCardFrameRectTransform.anchoredPosition,0.3f)
            .OnComplete(() =>
            {
                if (combiningCardCount == 2)
                {
                    InstantiateCombinedCard();
                }
            });
    }
    
    //RightCardFrame에 RightCard를 놓는 함수
    public void PlaceOnRightCardFrame(GameObject rightCardObject)
    {
        _rightCard = rightCardObject;
        _isPlacedRightCard = true;
        combiningCardCount++;
        
        RectTransform rightCardRectTransform = rightCardObject.GetComponent<RectTransform>();
        rightCardRectTransform.DOAnchorPos(rightCardFrameRectTransform.anchoredPosition,0.3f)
            .OnComplete(() =>
            {
                if (combiningCardCount >= 2)
                {
                    InstantiateCombinedCard();
                }
            });
    }

    private void InstantiateCombinedCard()
    {
        CardData combiningCardData = ScriptableObject.CreateInstance<CardData>();
        
        combiningCardData.SetCombiningCardData(_leftCard.GetComponent<Card>().CardData.LeftCardData,
            _rightCard.GetComponent<Card>().CardData.RightCardData);
        
        cardHand.AddCard(true, cardPrefab,combiningCardData, cardFrameRectTransform.anchoredPosition, true);
        leftCards.Remove(_leftCard);
        rightCards.Remove(_rightCard);
        Destroy(_leftCard);
        Destroy(_rightCard);
        _leftCard = null;
        _rightCard = null;
        _isPlacedLeftCard = false;
        _isPlacedRightCard = false;

        combiningCardCount = 0;
    }
}
