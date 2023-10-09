using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Inst { get; private set; }

    private GameState currentGameState;

    [SerializeField]
    private BeforeGameState beforeGameState;
    
    void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        ChangeGameState(beforeGameState);
    }
    
    //State를 변경한 후 해당 State를 실행한다.
    public void ChangeGameState(GameState gameState)
    {
        currentGameState = gameState;
        currentGameState.Execute();
    }
}
