using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        Loading,
        UI,
        Die,
    }

    public bool isStop = false;
    public bool isDead = false;
    private GameState gameState = GameManager.GameState.Playing;
    public GameState gamestate => gameState;

    public int lastStage = 0;
    
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
            isStop = true;
        }
        else if(gameState == GameState.Die)
        {
            //lastStage = currentStage;
            isDead = true;
            //SceneChangeManager.instance.SceneChange(SceneChangeManager.SceneType.Die); 뒤지는 씬으로 넘기기
        }
        else if(gameState == GameState.Playing)
        {
            isStop = false;
            isDead = false;
        }
        else if(gamestate == GameState.Loading)
        {
            isStop = true;
        }
    }
}
