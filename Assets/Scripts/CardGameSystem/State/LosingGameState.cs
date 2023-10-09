using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LosingGameState : MonoBehaviour,GameState
{
    public void Execute()
    {
        ShowLosingText();
    }
    [SerializeField]
    private TextMeshProUGUI losingText;

    private void ShowLosingText()
    {
        losingText.gameObject.SetActive(true);
        losingText.rectTransform.DOScale(Vector3.one, 0.7f);
        losingText.rectTransform.DOScale(Vector3.zero, 0.7f)
            .SetDelay(1f);
    }
}
