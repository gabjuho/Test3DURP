using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHand : MonoBehaviour
{
    [SerializeField]
    private List<Card> handCards;
    public List<Card> HandCards => handCards;
    
    [SerializeField] private RectTransform cardSpawnPoint; // 카드를 뽑는 위치 (덱)
    [SerializeField] private RectTransform cardLeft; // 제일 왼쪽에 있는 카드의 Tr
    [SerializeField] private RectTransform cardRight; // 제일 오른쪽에 있는 카드의 Tr

    [SerializeField,Range(1f, 2f)]
    private float cardSizeFactor = 1.3f;
    
    public static GameObject CurrentCardField { get; set; }

    //덱에서부터 카드를 뽑는 함수
    public void AddCardFromDeck(bool isMine, GameObject cardPrefab, CardData cardData)
    {
        GameObject cardObject = Instantiate(cardPrefab,cardSpawnPoint.position, Utils.QI, gameObject.transform);

        //isMine을 쓰는 것은 달갑지 않고
        //Image제어 즉, 활성화 비활성화는 CardUI에 함수로 변경 요망
        if (!isMine)
        {
            Image cardImage = cardObject.GetComponent<Image>();
            cardImage.raycastTarget = false;
        }
        
        Card card = cardObject.GetComponent<Card>();
        card.SetCard(cardData);
        
        card.Setup(isMine);
        handCards.Add(card);
        
        SetOriginOrder();
        CardAlignment();
    }

    //원하는 위치에서 카드를 추가해 손패로 옮겨주는 함수
    public void AddCard(bool isMine, GameObject cardPrefab, CardData cardData, Vector3 spawnPosition, bool isCombined = false)
    {
        if (cardData == null)
        {
            Debug.Log("cardData가 null입니다.");
        }
            
        GameObject cardObject = Instantiate(cardPrefab,spawnPosition, Utils.QI, gameObject.transform);
        //isMine을 쓰는 것은 달갑지 않고
        //Image제어 즉, 활성화 비활성화는 CardUI에 함수로 변경 요망
        if (!isMine)
        {
            Image cardImage = cardObject.GetComponent<Image>();
            cardImage.raycastTarget = false;
        }
        
        Card card = cardObject.GetComponent<Card>();
        card.IsCombined = isCombined;
        card.SetCard(cardData);
 
        card.Setup(isMine);
        handCards.Add(card);
        
        SetOriginOrder();
        CardAlignment();
    }

    //손패에서 카드를 제거하는 함수
    public void RemoveCard(GameObject cardObject, bool isMine)
    {
        Card card = cardObject.GetComponent<Card>();
        handCards.Remove(card);
        SetOriginOrder();
        CardAlignment();
    }

    private void SetOriginOrder()
    {
        int count = handCards.Count;
        for (int i = 0; i < count; i++)
        {
            Card targetCard = handCards[i];
            targetCard.GetComponent<CardOrder>().SetOriginOrder(i);
        }
    }

    private void CardAlignment()
    {
        List<PRS> originCardPRSs = new List<PRS>();
        
            // 내 패 정렬
            originCardPRSs = RoundAlignment(cardLeft, cardRight, handCards.Count, 0.5f, Vector3.one * cardSizeFactor);

            
        List<Card> targetCards = handCards;
        for (int i = 0; i < targetCards.Count; i++)
        {
            Card targetCard = targetCards[i];

            targetCard.originPRS = originCardPRSs[i];
            targetCard.MoveTransform(targetCard.originPRS, true, 0.7f);
        }
    }

    private List<PRS> RoundAlignment(RectTransform leftTr, RectTransform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount);

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
            Vector3 targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            Quaternion targetRot = Utils.QI;
            if (objCount >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                curve = height >= 0 ? curve : -curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }

        return results;
    }
}
