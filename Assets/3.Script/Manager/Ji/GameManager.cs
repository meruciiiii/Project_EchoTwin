using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        UI,
        Die,
    }

    public int timeScale = 1;
    private GameState gameState = GameManager.GameState.Playing;
    public GameState gamestate => gameState;

    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeState(GameState state)
    {
        if (state == gameState) return;

        gameState = state;
        Debug.Log(gameState);
        if(gameState == GameState.UI)
        {
            timeScale = 0;
        }
        else if(gameState == GameState.Die)
        {
            timeScale = 0;
        }
    }
}
