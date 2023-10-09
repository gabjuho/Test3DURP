using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    private int health;
    public int Health => health;

    [SerializeField]
    private PlayerData playerData;

    private PlayerUI playerUI;

    private void Awake()
    {
        health = playerData.Health;
        playerUI = GetComponent<PlayerUI>();
    }

    private void Start()
    {
        playerUI.UpdatePlayerUI();
    }

    //플레이어가 데미지를 받는 함수
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
            Die();
        }
        
        playerUI.UpdatePlayerUI();
    }

    protected abstract void Die();
}
