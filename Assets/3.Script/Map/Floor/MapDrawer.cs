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
    public void EnterDraw(IReadOnlyDictionary<Vector2Int, FloorData> microMap)
    {
        RemoveMapNode();
        DrawMap(microMap);
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
    private void DrawMap(IReadOnlyDictionary<Vector2Int, FloorData> microMap)
    {
        if (!TryGetComponent(out parent))
            Debug.Log("TryGetComponent RectTransform parent is fail");
        plateList = new List<GameObject>();
        foreach (KeyValuePair<Vector2Int, FloorData> drawMap in microMap)
        {
            GameObject ui = Instantiate(plate, parent);
            plateList.Add(ui);
            plateMappings.Add(drawMap.Key, ui);
            // Instantiate 이후 루프 안에서...
            //MapNodeUI nodeUI = ui.GetComponent<MapNodeUI>(); // 커스텀 스크립트가 있다면
            Image[] images = ui.GetComponentsInChildren<Image>();
            for (int i = 0; i < 4; i++)
            {
                // 각 방 데이터의 isOpen 상태에 따라 문 UI 활성화/비활성화
                images[i + 1].enabled = drawMap.Value.GetDoorState(i);
            }
            // drawMap 데이터 중 startroom 값이 true 이면 green으로 색상 변경
            if (drawMap.Value.getBoolStartRoom())
            {
                //Debug.Log(drawMap.Key+": start node");
                images[0].color = Color.red;
            }
            // drawMap 데이터 중 endroom 값이 true 이면 red으로 색상 변경
            if (drawMap.Value.getBoolEndRoom())
            {
                //Debug.Log(drawMap.Key + ": end node");
                //bossRoomisTop = drawMap.Value.GetDoorState(2);
                //Debug.Log("bossssssssssssssssssssssssssRoomisTop is " + bossRoomisTop);
                images[0].color = Color.green;
            }
            RectTransform rt = ui.GetComponent<RectTransform>();

            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);

            rt.anchoredPosition = drawMap.Key * 100;
        }
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
