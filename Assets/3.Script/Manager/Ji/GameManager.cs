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

    //저장될 정보들----------------
    public int lastStage = 0;
    public int playerGold = 0;
    public int playerCristal = 0;
    public int maxHP = 0;
    public int currentHP = 0;
    //-----------------------------

    public event Action<Vector3, Vector2Int> whenGoNextMap;
    private Vector2Int currentCell;
    private IReadOnlyDictionary<Vector2Int, List<GameObject>> enemieDic;

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

    public void setDic(IReadOnlyDictionary<Vector2Int, List<GameObject>> dic)
    {
        enemieDic = dic;
    }

    public void whenMapChange(Vector3 destPos, Vector2Int dicKey)
    {
        currentCell = dicKey;
        ChangeState(GameState.Loading);

        whenGoNextMap?.Invoke(destPos, dicKey);
    }

    public void whenPlayerArrived(Vector2Int dicKey)
    {
        setEnemyActive(dicKey);
        ChangeState(GameState.Playing);
    }

    private void setEnemyActive(Vector2Int currentCell)
    {
        if (enemieDic == null) return;
        if (!enemieDic.TryGetValue(currentCell, out List<GameObject> list) || list == null) return;

        for(int i=0; i<list.Count;i++)
        {
            if(list[i] != null)
            {
                EnemyStateAbstract enemy = list[i].GetComponent<EnemyStateAbstract>();
                enemy.state = EnemyState.chase;
            }
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
            //Time.timeScale = 0f;
        }
        else if(gameState == GameState.Die)
        {
            isDead = true;
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
