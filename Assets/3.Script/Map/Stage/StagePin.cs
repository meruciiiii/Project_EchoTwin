
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePin : MonoBehaviour
{
    private GameObject pinPlayer;
    private Transform currentNode;
    /// <summary>
    /// 현재 플레이어가 진입했던 위치를 가지고 있다
    /// 다음 목적지를 선택하면 목적지까지 이동한다
    /// 목적지를 선택하면 선택된 목적지의 오브젝트와 포지션을 가져와
    /// 현재 위치로 부터 방향과 거리를 잡고 이동한다
    /// 목적지는 Floor에 나와있는 포지션 보다 -20 y 이동시킨다
    /// 이동이 완료 되면 해당 노드의 타입에 맞게 맵을 이동한다
    /// </summary>
    
    [SerializeField] private float moveSpeed = 5f;
    public void CanMove(NodeData nodeData)
    {
        StartCoroutine(MoveTo(nodeData));
    }
    public IEnumerator MoveTo(NodeData nodeData)
    {
        Vector3 targetPos = nodeData.positionTarget.transform.position;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

            yield return null;
        }
    }
}
