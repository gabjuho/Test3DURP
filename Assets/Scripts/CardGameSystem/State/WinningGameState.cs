using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class WinningGameState : MonoBehaviour,GameState
{
    [SerializeField]
    private TextMeshProUGUI winningText;
    public void Execute()
    {
        ShowWinningText();
    }

    private void ShowWinningText()
    {
        winningText.gameObject.SetActive(true);
        winningText.rectTransform.DOScale(Vector3.one, 0.7f);
        winningText.rectTransform.DOScale(Vector3.zero, 0.7f)
            .SetDelay(1f);
    }
}
