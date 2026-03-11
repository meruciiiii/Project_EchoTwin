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
        Clear,
        RoomClear,
    }

    public bool isStop = false;
    public bool isDead = false;
    public bool isGetWeapon = false;
    [SerializeField] private GameState gameState = GameManager.GameState.Playing;
    public GameState gamestate => gameState;

    //ÀúÀåµÉ Á¤º¸µé----------------
    public int lastStage = 0;
    public int playerGold = 0;
    public int playerCristal = 0;
    public int maxHP = 0;
    public int currentHP = 0;
    //-----------------------------

    public event Action<Vector3, Vector2Int> whenGoNextMap;
    public event Action whenArriveNextMap;
    public event Action whenNodeClear;
    public event Action roomClearCheck;
    private Vector2Int currentCell;
    private IReadOnlyDictionary<Vector2Int, List<GameObject>> enemyDic;

    public event Action<WeaponAbstract, WeaponAbstract> turnWeaponUI;
    public event Action setResonanceUI;
    public event Action GetWeapon;
    public int monsterCount = 0;
    public event Action playerDie;
    public List<GameObject> ItemList = new List<GameObject>();

    private WeaponAbstract MainWeapon;
    private WeaponAbstract SubWeapon;

    public WeaponAbstract mainWeapon => MainWeapon;
    public WeaponAbstract subWeapon => SubWeapon;

    public event Action CameraZoomOut;
    public event Action CameraReset;

    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ZoomOutEvent()
    {
        CameraZoomOut?.Invoke();
    }

    public void CameraResetEvent()
    {
        CameraReset?.Invoke();
    }

    public void AddItemToList(GameObject obj)
    {
        if (obj == null) return;
        if (!ItemList.Contains(obj))
        {
            ItemList.Add(obj);
        }
    }

    public void RemoveItemFromList(GameObject obj)
    {
        if (obj == null) return;
        ItemList.Remove(obj);
    }

    private void ClearItemList()
    {
        for (int i = ItemList.Count - 1; i >= 0; i--)
        {
            if (ItemList[i] != null)
            {
                Destroy(ItemList[i]);
            }
        }

        ItemList.Clear();
    }

    public void setCountInRoom()
    {
        monsterCount = 0;

        if (enemyDic == null) return;
        if (!enemyDic.TryGetValue(currentCell, out List<GameObject> list) || list == null) return;

        for (int i = 0; i < list.Count; i++)
        {
            GameObject roomEnemy = list[i];
            if (roomEnemy == null) continue;

            EnemyStateAbstract enemy = roomEnemy.GetComponent<EnemyStateAbstract>();
            if (enemy == null) continue;
            if (enemy.state == EnemyState.dead) continue;

            monsterCount++;
        }
    }

    public void checkCountInRoom()
    {
        if (gamestate == GameState.Die) return;
        if (gamestate == GameState.RoomClear) return;
        if (gamestate == GameState.Clear) return;

        monsterCount--;

        if (monsterCount <= 0)
        {
            monsterCount = 0;
            ChangeState(GameState.RoomClear);
            roomClearCheck?.Invoke();
        }
    }

    public void TurnWeaponUI(WeaponAbstract mainWeapon, WeaponAbstract subWeapon)
    {
        MainWeapon = mainWeapon;
        SubWeapon = subWeapon;

        isGetWeapon = (mainWeapon != null);
        
        turnWeaponUI?.Invoke(mainWeapon, subWeapon);
        if (mainWeapon != null) GetWeapon?.Invoke();
    }

    public void setResonance()
    {
        setResonanceUI?.Invoke();
    }

    public void setDic(IReadOnlyDictionary<Vector2Int, List<GameObject>> dic)
    {
        enemyDic = dic;
    }

    public void whenMapChange(Vector3 destPos, Vector2Int dicKey)
    {
        currentCell = dicKey;
        monsterCount = 0;
        ChangeState(GameState.Loading);

        whenGoNextMap?.Invoke(destPos, dicKey);
    }

    public void whenPlayerArrived(Vector2Int dicKey)
    {
        currentCell = dicKey;
        setEnemyActive(dicKey);
        setCountInRoom();

        if (monsterCount <= 0)
        {
            ChangeState(GameState.RoomClear);
            roomClearCheck?.Invoke();
        }
        else
        {
            ChangeState(GameState.Playing);
        }

        whenArriveNextMap?.Invoke();
    }

    private void setEnemyActive(Vector2Int currentCell)
    {
        if (enemyDic == null) return;
        if (!enemyDic.TryGetValue(currentCell, out List<GameObject> list) || list == null) return;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null)
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
        //Debug.Log(gameState);
        if (gameState == GameState.UI)
        {
            isStop = true;
            //Time.timeScale = 0f;
        }
        else if (gameState == GameState.Die)
        {
            isStop = true;
            isDead = true;
            playerDie?.Invoke();
            GetCurrency.destroyAllHeart();
            ClearItemList();
        }
        else if (gameState == GameState.Playing)
        {
            isStop = false;
            isDead = false;
        }
        else if (gamestate == GameState.Loading)
        {
            isStop = true;
        }
        else if (gamestate == GameState.Clear)
        {
            isStop = true;
            whenNodeClear?.Invoke();
            GetCurrency.destroyAllHeart();
            ClearItemList();
        }
        else if (gamestate == GameState.RoomClear)
        {
            isStop = false;
            isDead = false;
        }
    }
}
