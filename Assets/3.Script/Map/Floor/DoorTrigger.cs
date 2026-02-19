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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Door Triger is started");
        if(Direction== Vector2Int.zero)
        {
            Debug.Log("Direction is not selected");
            return;
        }
        Debug.Log("Invoke from: " + gameObject.name + " / " + GetInstanceID());
        Debug.Log("Invoke : " + this.GetInstanceID());
        onPlayerEnter?.Invoke(Direction);
    }
}
