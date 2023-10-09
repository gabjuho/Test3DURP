using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CardRemovingButton : MonoBehaviour
{
    private Button cardRemovingButton;

    private void Awake()
    {
        cardRemovingButton = GetComponent<Button>();
    }

    //CardRemovingButton을 비활성화하는 함수
    public void BeDisableButton()
    {
        gameObject.SetActive(false);
    }
}
