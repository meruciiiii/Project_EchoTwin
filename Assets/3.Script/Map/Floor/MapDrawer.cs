using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapDrawer : MonoBehaviour
{
    public GameObject plate;
    private RectTransform parent;
    private List<GameObject> plateList;
    private BiDictionary<Vector2Int, GameObject> plateMappings;
    public void EnterDraw(IReadOnlyDictionary<Vector2Int, FloorData> microMap, Vector2Int currentCoord)
    {
        RemoveMapNode();
        DrawMap(microMap, currentCoord);
    }
    private void RemoveMapNode()
    {
        if (plateList == null) return;
        //Debug.Log("plateList : " + plateList.Count);
        foreach (GameObject gameObject in plateList)
        {
            Destroy(gameObject);
        }
        plateList?.Clear();
        plateMappings?.Clear();
    }
    private void DrawMap(IReadOnlyDictionary<Vector2Int, FloorData> microMap, Vector2Int currentCoord)
    {
        if (!TryGetComponent(out parent))
            Debug.Log("TryGetComponent RectTransform parent is fail");
        plateList = new List<GameObject>();
        plateMappings = new BiDictionary<Vector2Int, GameObject>();
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                FloorData floorData = null;
                Vector2Int plateVector = currentCoord + (i - 1) * Vector2Int.left + (j - 1) * Vector2Int.up;
                if (microMap.ContainsKey(plateVector))
                {
                    floorData = microMap[plateVector];
                }
                else
                {
                    continue;
                }
                GameObject ui = Instantiate(plate, parent);
                plateList.Add(ui);
                plateMappings.Add(plateVector, ui);

                Image[] images = ui.GetComponentsInChildren<Image>();
                for (int k = 0; k < 4; k++)
                {
                    images[k + 1].enabled = floorData.GetDoorState(k);
                }
                if (floorData.GetVisit())
                {
                    images[0].color = Color.white;
                }
                else
                {
                    images[0].color = Color.gray;
                }
                RectTransform rt = ui.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(parent.anchoredPosition.x + 197, parent.anchoredPosition.y+ 152) + (i - 1) * 100 * Vector2.left + (j - 1) * 70 * Vector2.up;
            }
        }
        ///foreach (KeyValuePair<Vector2Int, FloorData> drawMap in microMap)
        ///{
        ///    GameObject ui = Instantiate(plate, parent);
        ///    plateList.Add(ui);
        ///    plateMappings.Add(drawMap.Key, ui);
        ///    // Instantiate 이후 루프 안에서...
        ///    //MapNodeUI nodeUI = ui.GetComponent<MapNodeUI>(); // 커스텀 스크립트가 있다면
        ///    Image[] images = ui.GetComponentsInChildren<Image>();
        ///    for (int i = 0; i < 4; i++)
        ///    {
        ///        // 각 방 데이터의 isOpen 상태에 따라 문 UI 활성화/비활성화
        ///        images[i + 1].enabled = drawMap.Value.GetDoorState(i);
        ///    }
        ///    // drawMap 데이터 중 startroom 값이 true 이면 green으로 색상 변경
        ///    if (drawMap.Value.getBoolStartRoom())
        ///    {
        ///        //Debug.Log(drawMap.Key+": start node");
        ///        images[0].color = Color.green;
        ///    }
        ///    // drawMap 데이터 중 endroom 값이 true 이면 red으로 색상 변경
        ///    if (drawMap.Value.getBoolEndRoom())
        ///    {
        ///        //Debug.Log(drawMap.Key + ": end node");
        ///        //bossRoomisTop = drawMap.Value.GetDoorState(2);
        ///        //Debug.Log("bossssssssssssssssssssssssssRoomisTop is " + bossRoomisTop);
        ///        images[0].color = Color.red;
        ///    }
        ///    RectTransform rt = ui.GetComponent<RectTransform>();
        ///    rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        ///    rt.anchoredPosition = new Vector2(drawMap.Key.x * 100, drawMap.Key.y * 70);
        ///}
    }
    public void AlreadyStep(Vector2Int playerFootprint)
    {
        // ui 변수를 선언함과 동시에 TryGetValue의 결과가 true일 때만 로직 실행
        if (plateMappings != null && plateMappings.TryGetValue(playerFootprint, out GameObject ui))
        {
            // ui가 확실히 존재할 때만 컴포넌트를 가져옴
            Image[] images = ui.GetComponentsInChildren<Image>();

            if (images != null && images.Length > 0)
            {
                // 지나간 plate가 되면 회색으로 변환
                images[0].color = Color.gray;
            }
        }
        else
        {
            Debug.LogWarning($"{playerFootprint} 위치의 발판을 찾을 수 없습니다.");
        }
    }// 지나간 plate가 되면 회색으로 변환
    public void playerStanding(Vector2Int playerPosition)
    {
        // ui 변수를 선언함과 동시에 TryGetValue의 결과가 true일 때만 로직 실행
        if (plateMappings != null && plateMappings.TryGetValue(playerPosition, out GameObject ui))
        {
            // ui가 확실히 존재할 때만 컴포넌트를 가져옴
            Image[] images = ui.GetComponentsInChildren<Image>();

            if (images != null && images.Length > 0)
            {
                // 새로운 plate가 되면 노랑으로 변환
                images[0].color = Color.yellow;
            }
        }
        else
        {
            Debug.LogWarning($"{playerPosition} 위치의 발판을 찾을 수 없습니다.");
        }
    }// 새로운 plate가 되면 노랑으로 변환
}
