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
        //if(state == GameState.UI)
        //{
        //    timeScale = 0;
        //}
        //else if()
        //{
        //
        //}
    }
}
