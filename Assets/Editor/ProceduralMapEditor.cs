using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapDrawer))]
public class ProceduralMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapDrawer generator = (MapDrawer)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Map"))
        {

            //generator.SettingMap();
        }

        // 맵 삭제 버튼 로직 수정
        if (GUILayout.Button("Clear Map"))
        {
            ClearWorldMap();
            //
        }
    }

    private void ClearWorldMap()
    {
        // 1. "World_Map"이라는 이름을 가진 오브젝트를 하이어라키에서 직접 찾습니다.
        GameObject[] existingWorld = GameObject.FindGameObjectsWithTag("MapNode");

        if (existingWorld != null)
        {
            // 2. 에디터에서 삭제할 때는 Undo 시스템에 등록해야 Ctrl+Z가 가능하며,
            // Unity 6 환경에서 가장 안전한 삭제 방식입니다.
            foreach(GameObject target in existingWorld)
            Undo.DestroyObjectImmediate(target);
            //Debug.Log("World_Map이 삭제되었습니다.");
        }
        else
        {
            Debug.LogWarning("삭제할 World_Map을 찾을 수 없습니다.");
        }
    }
}