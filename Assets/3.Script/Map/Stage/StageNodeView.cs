using System;
using System.Collections.Generic;
using UnityEngine;

public class StageNodeView : MonoBehaviour
{
    [SerializeField] private NodeContainer nodeContainer;
    [SerializeField] private RectTransform linePrefab;
    [SerializeField] private RectTransform lineContainer;
    private StageDrawer stageDrawer;

    [SerializeField] private float lineThickness = 5f;
    public Action<NodeData> OnNodeClicked;

    private void Awake()
    {
        if (!TryGetComponent(out stageDrawer))
        {
            Debug.Log("TryGetComponent StageDrawer is fail");
        }
        stageDrawer.OnNodeClicked += node =>
        {
            OnNodeClicked?.Invoke(node);
        };
    }
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
                    if (!nodeContainer.floors[node.floorIndex].Floor[node.nodeIndex].positionTarget.TryGetComponent(out from))
                    {
                        Debug.Log("TryGetComponent RectTransform from is fail");
                    }
                    RectTransform to;
                    if (!nodeContainer.floors[next.floorIndex].Floor[next.nodeIndex].positionTarget.TryGetComponent(out to))
                    {
                        Debug.Log("TryGetComponent RectTransform to is fail");
                    }
                    DrawLine(from, to);
                }
            }
        }
        stageDrawer.Matching(floors, nodeContainer);
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
    }
}
