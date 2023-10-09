using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PlayerData", order = Int32.MaxValue, fileName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField]
    private int health;
    public int Health => health;
}