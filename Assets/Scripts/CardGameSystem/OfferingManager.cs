using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OfferingManager : MonoBehaviour
{
    public static OfferingManager Inst { get; private set; }

    //제물을 바치기 전에 낼 카드를 임시 위치로 옮겨두기 위한 RectTransform
    [SerializeField]
    private RectTransform tempCardRectTransform;

    [SerializeField]
    private CardFieldGroupController cardFieldGroupController;

    //소환할 카드
    private GameObject _summoningCardObject;
    private Card _summoningCard;
    private Image _summoningCardImage;
    private RectTransform _summoningCardRectTransform;
    private bool _isOfferingMode = false;
    public bool IsOfferingMode => _isOfferingMode;

    private bool _isEnoughOfferingCount = false;
    public bool IsEnoughOfferingCount =>_isEnoughOfferingCount;
    
    private int _offeringCount = 0;
    public int OfferingCount => _offeringCount;
    private List<GameObject> _offeringCardFieldObjects = new List<GameObject>();
    
    void Awake()
    {
        Inst = this;
    }

    //제물모드에 진입하는 함수
    public void EntryOfferingMode(GameObject summoningCardObject)
    {
        _summoningCardObject = summoningCardObject;

        _summoningCard = _summoningCardObject.GetComponent<Card>();
        _summoningCardRectTransform = _summoningCardObject.GetComponent<RectTransform>();
        _summoningCardImage = _summoningCardObject.GetComponent<Image>();
        
        //모든 필드를 비활성화한 후에 카드가 존재하는 필드만 활성화시킨다. (비어있는 필드가 활성화되어있기 때문에 이들을 비활성화 시킨 후에 활성화해야한다.)
        cardFieldGroupController.ActivateMyCardFieldOnlyHavingCard();
        
        _summoningCardImage.raycastTarget = false;
        _summoningCardRectTransform.DOAnchorPos(tempCardRectTransform.anchoredPosition, 0.4f);
        _summoningCardRectTransform.DORotateQuaternion(Quaternion.identity, 0.4f);

        _isOfferingMode = true;
    }

    //제물 모드 때 필요없는 UI 비활성화
    public void InActivateUnnecessaryUI()
    {
        
    }

    //제물로 바칠 카드를 선택하는 함수
    public void RegisterOfferingCard(GameObject cardFieldObject)
    {
        CardField cardField = cardFieldObject.GetComponent<CardField>();
        CanvasGroup cardCanvasGroup = cardField.CardOnField.GetComponent<CanvasGroup>();
        cardCanvasGroup.alpha = 0.3f;

        Card card = cardField.CardOnField.GetComponent<Card>();

        if (card.CardData.LeftCardData.AbilityType == AbilityTable.EAbilityName.Sacrifice2 ||
            card.CardData.LeftCardData.AbilityType == AbilityTable.EAbilityName.Sacrifice3
            )
        {
            Sacrifice sacrifice = AbilityTable.AbilityDictionary[card.CardData.LeftCardData.AbilityType] as Sacrifice;
            _offeringCount += sacrifice.SacrificingCount;
        }
        else if (card.CardData.RightCardData.AbilityType == AbilityTable.EAbilityName.Sacrifice2 ||
                 card.CardData.RightCardData.AbilityType == AbilityTable.EAbilityName.Sacrifice3)
        {
            Sacrifice sacrifice = AbilityTable.AbilityDictionary[card.CardData.RightCardData.AbilityType] as Sacrifice;
            _offeringCount += sacrifice.SacrificingCount;
        }
        else
        {
            _offeringCount += 1;
        }
        
        _offeringCardFieldObjects.Add(cardFieldObject);
        
        if (_offeringCount >= _summoningCard.OfferingCount)
        {
            foreach (GameObject cardObject in _offeringCardFieldObjects)
            {
                CardField tempCardField = cardObject.GetComponent<CardField>();
                Card tempCard = tempCardField.CardOnField.GetComponent<Card>();
                tempCard.KillCard();
                tempCardField.RemoveCard();
            }
            _offeringCardFieldObjects.Clear();
            _offeringCount = _offeringCardFieldObjects.Count;
            _isEnoughOfferingCount = true;
            
            cardFieldGroupController.ActivateMyEmptyCardField();
        }
    }

    //제물로 바쳐질 카드를 등록 해제하는 함수
    public void UnRegisterOfferingCard(GameObject cardFieldObject)
    {
        CardField cardField = cardFieldObject.GetComponent<CardField>();
        CanvasGroup cardCanvasGroup = cardField.CardOnField.GetComponent<CanvasGroup>();
        cardCanvasGroup.alpha = 1f;
        
        _offeringCardFieldObjects.Remove(cardFieldObject);
        _offeringCount = _offeringCardFieldObjects.Count;
    }

    //제물을 바친 후 선택된 CardField에 카드를 소환하는 함수
    public void SummonCardOnMyCardField(Vector3 cardFieldAnchoredPosition,RectTransform cardFieldRectTransform)
    {
        CardHandler summoningCardHandler = _summoningCardObject.GetComponent<CardHandler>();
        
        summoningCardHandler.PutCardOnMyField(cardFieldAnchoredPosition, cardFieldRectTransform);
        ExitOfferingMode();
    }

    //제물 모드를 나가는 함수
    public void ExitOfferingMode()
    {
        cardFieldGroupController.SetCardFieldUIIsOfferingToFalse();
        _isOfferingMode = false;
        _isEnoughOfferingCount = false;
    }
}
