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
    private bool bossRoomisTop = false;
    public void EnterDraw(IReadOnlyDictionary<Vector2Int, MapData> microMap)
    {
        RemoveMapNode();
        DrawMap(microMap);
    }
    //private void SettingMap(IReadOnlyDictionary<Vector2Int, MapData> microMap)
    //{
    //    //int plateCount = 0;
    //    //int minCount = 15;
    //    //int maxCount = 15;
    //    //int safetyCount = 50;
    //    //do
    //    //{
    //        RemoveMapNode();
    //        DrawMap(microMap);
    //    //plateCount = CountObject();
    //    //if (!BossRoomCheck())
    //    //plateCount = 0;
    //    //safetyCount--;
    //    //if (safetyCount < 0)
    //    //        {
    //    //minCount = 14;
    //    //maxCount = 16;
    //    //}
    //    //if (safetyCount < -40)
    //    //{
    //    //Debug.Log("is infinity");
    //    //break;
    //    //}
    //    //}
    //    //Debug.Log("bossRoomisTop is infinity" + BossRoomCheck());
    //    //}
    //    //while (plateCount < minCount || plateCount > maxCount);
    //}
    //private bool BossRoomCheck()
    //{
    //    // 추후에 bossRoomisTop 반환 할 예정
    //    return bossRoomisTop;
    //}
    //private int CountObject()
    //{
    //    return plateList.Count;
    //}
    private void RemoveMapNode()
    {
        if (plateList == null) return;
        //Debug.Log("plateList : " + plateList.Count);
        foreach (GameObject gameObject in plateList)
        {
            Destroy(gameObject);
        }
    }
    private void DrawMap(IReadOnlyDictionary<Vector2Int, MapData> microMap)
    {
        if (!TryGetComponent(out parent))
            Debug.Log("TryGetComponent RectTransform parent is fail");
        bossRoomisTop = false;
        plateList = new List<GameObject>();
        foreach (KeyValuePair<Vector2Int, MapData> drawMap in microMap)
        {
            GameObject ui = Instantiate(plate, parent);
            plateList.Add(ui);
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
}
