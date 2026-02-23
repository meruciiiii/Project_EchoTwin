using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DoorTrigger : MonoBehaviour
{
    public enum MoveDirection
    {
        Right,
        Left,
        Down,
        Up
    }
    public MoveDirection selectedDoorDirection;
    public int doorIntDirection
    {
        get
        {
            switch (selectedDoorDirection)
            {
                case MoveDirection.Right: return 0;
                case MoveDirection.Left: return 1;
                case MoveDirection.Down: return 2;
                case MoveDirection.Up: return 3;
                default: return -1;
            }
        }
    }// 0 : East, 1 : West, 2 : South, 3 : North
    public Vector2Int Direction
    {
        get
        {
            switch (selectedDoorDirection)
            {
                case MoveDirection.Right: return Vector2Int.right;
                case MoveDirection.Left: return Vector2Int.left;
                case MoveDirection.Down: return Vector2Int.down;
                case MoveDirection.Up: return Vector2Int.up;
                default: return Vector2Int.zero;
            }
        }
    }
    public Action<Vector2Int> onPlayerEnter;

    private bool isLocked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isLocked) return;
        if (!other.CompareTag("Player")) return;
        //Debug.Log("Door Triger is started");
        if(Direction== Vector2Int.zero)
        {
            Debug.Log("Direction is not selected");
            return;
        }
        onPlayerEnter?.Invoke(Direction);
    }
    public void Lock(float duration)
    {
        StartCoroutine(LockRoutine(duration));
    }

    private IEnumerator LockRoutine(float duration)
    {
        isLocked = true;
        yield return new WaitForSeconds(duration);
        isLocked = false;
    }
}
