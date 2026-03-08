using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageNodeView : MonoBehaviour
{
    [SerializeField] private TransformGroup[] positionTargets;
    [SerializeField] private GameObject[] nodePrefabs = new GameObject[6];
    [SerializeField] private RectTransform linePrefab;
    [SerializeField] private RectTransform lineContainer;
    [SerializeField] private float lineThickness = 5f;
    private Dictionary<StageNode, GameObject> nodeViews = new Dictionary<StageNode, GameObject>();
    [SerializeField] private GameObject playerPin;
    private StageNode currentNode;
    public event Action<StageNode> onNodeEntered;
    private Coroutine moveRoutine;
#if UNITY_EDITOR
    [ContextMenu("NodeContainer/positionTargets Create")]
    private void TargetsSetting()
    {
        positionTargets = new TransformGroup[6];
        for (int i = 0; i < positionTargets.Length; i++)
        {
            positionTargets[i] = new TransformGroup();
        }
        positionTargets[0].targets = new Transform[1];
        positionTargets[1].targets = new Transform[2];
        positionTargets[2].targets = new Transform[3];
        positionTargets[3].targets = new Transform[4];
        positionTargets[4].targets = new Transform[4];
        positionTargets[5].targets = new Transform[1];
    }
#endif
    public void DrawConnections(List<List<StageNode>> floors)
    {
        ClearLines();

        for (int floor = 0; floor < floors.Count - 1; floor++)
        {
            foreach (StageNode node in floors[floor])
            {
                foreach (StageNode next in node.nextNodes)
                {
                    RectTransform from;
                    if (!positionTargets[node.floorIndex].targets[node.nodeIndex].TryGetComponent(out from))
                    {
                        Debug.Log("TryGetComponent RectTransform from is fail");
                    }
                    RectTransform to;
                    if (!positionTargets[next.floorIndex].targets[next.nodeIndex].TryGetComponent(out to))
                    {
                        Debug.Log("TryGetComponent RectTransform to is fail");
                    }
                    DrawLine(from, to);
                }
            }
        }
        Draw(floors);
    }
    private void Draw(List<List<StageNode>> floors)
    {
        ClearTargetNode();

        for (int i = 0; i < floors.Count; i++)
        {
            for (int j = 0; j < floors[i].Count; j++)
            {
                StageNode node = floors[i][j];

                Transform target = positionTargets[i].targets[j];

                GameObject obj = Instantiate(nodePrefabs[(int)node.nodeType], target.position, target.rotation, target );

                nodeViews.Add(node, obj);

                Button btn;
                if (!obj.TryGetComponent(out btn))
                {
                    Debug.Log("TryGetComponent Button is fail");
                }
                else
                {
                    btn.onClick.AddListener(() => OnNodeClicked(node));
                }
            }
        }
        if(floors[0][0].nodeType.Equals(NodeType.Clear))
        currentNode = floors[0][0];
    }
    private void DrawLine(RectTransform from, RectTransform to)
    {
        Vector3 worldStart = from.TransformPoint(from.rect.center);
        Vector3 worldEnd = to.TransformPoint(to.rect.center);

        Vector2 start = lineContainer.InverseTransformPoint(worldStart);
        Vector2 end = lineContainer.InverseTransformPoint(worldEnd);

        Vector2 dir = end - start;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        RectTransform line = Instantiate(linePrefab, lineContainer);

        line.anchoredPosition = start;
        line.sizeDelta = new Vector2(distance, lineThickness);
        line.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void ClearLines()
    {
        foreach (Transform child in lineContainer)
        {
            Destroy(child.gameObject);
        }
        playerPin.transform.position = new Vector3(960, 170, 0);
    }
    private void ClearTargetNode()
    {
        if(nodeViews!=null)
        foreach (GameObject nodeObj in nodeViews.Values)
        {
            if (nodeObj != null)
            {
                Destroy(nodeObj);
            }
        }
        nodeViews.Clear();
    }
    private void OnNodeClicked(StageNode node)
    {
        if (!CanMove(node))
            return;

        MovePlayerPin(node);
    }
    private bool CanMove(StageNode target)
    {
        if (currentNode == null)
            return target.nodeType == NodeType.Clear;

        return currentNode.nextNodes.Contains(target);
    }
    private void MovePlayerPin(StageNode node)
    {
        if (currentNode == node)
            return;
        if(moveRoutine !=null)
            StopCoroutine(moveRoutine);
        else
            moveRoutine = StartCoroutine(MoveRoutine(node));
    }
    private IEnumerator MoveRoutine(StageNode targetNode)
    {
        StageNode startNode = currentNode;
        Debug.Log("next floor is " + targetNode.floorIndex);
        currentNode = targetNode;

        Vector3 startPos = nodeViews[startNode].transform.position - 20 * Vector3.down;
        Vector3 endPos = nodeViews[targetNode].transform.position - 20 * Vector3.down;

        float duration = 0.8f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            
            t = Mathf.SmoothStep(0, 1, t);

            playerPin.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        playerPin.transform.position = endPos;
        moveRoutine = null;
        ExecuteNodeEvent(currentNode);
    }
    private void ExecuteNodeEvent(StageNode node)
    {
        GameManager.instance.ChangeState(GameManager.GameState.Playing);
        onNodeEntered?.Invoke(node);
        //StageNodeMapClose();
    }
    private void StageNodeMapClose()
    {
        gameObject.SetActive(false);
    }
}
[System.Serializable]
public class TransformGroup
{
    public Transform[] targets;
}