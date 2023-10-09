using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    private Player player;
    [SerializeField]
    private TextMeshProUGUI healthText;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    //Player UI를 갱신하는 함수 (플레이어 HP)
    public void UpdatePlayerUI()
    {
        healthText.text = player.Health.ToString();
    }
}
