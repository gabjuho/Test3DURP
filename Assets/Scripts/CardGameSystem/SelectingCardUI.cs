using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectingCardUI : MonoBehaviour,IPointerClickHandler
{
    [SerializeField]
    private GameObject removingImage;

    private RectTransform selectingCardRectTransform;

    private bool willRemove = false;
    public bool WillRemove => willRemove;

    private void Awake()
    {
        selectingCardRectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (removingImage.activeSelf)
        {
            willRemove = false;
            removingImage.SetActive(false);
        }
        else
        {
            willRemove = true;
            removingImage.SetActive(true);
        }
    }

    //제안 카드를 스스로 파괴하는 함수 (오브젝트가 스스로 파괴하는 것은 좋은 구조가 아니다.)
    public void RemoveSelectingCardSelf()
    {
        selectingCardRectTransform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
