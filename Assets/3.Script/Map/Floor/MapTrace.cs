
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrace : MonoBehaviour
{
    private HashSet<Vector2Int> playerFootprint = new HashSet<Vector2Int>();
    private Vector2Int playerStayPoint = new Vector2Int(-20,-20);
    public Vector2Int GetCurrentStayPoint()
    {
        return playerStayPoint;
    }
    public void PlayerStep(Vector2Int stepRoom)
    {
        // UI 어두워지기 활성화 
        
        playerStayPoint = stepRoom;
        playerFootprint.Add(stepRoom);
        // UI 활성화 - 지도에서 이미
    }
}
