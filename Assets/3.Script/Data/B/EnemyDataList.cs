#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Map/EnemyDataList")]
public class EnemyDataList : ScriptableObject
{
    public List<EnemyData> enemyDatas;
#if UNITY_EDITOR
    [ContextMenu("Auto Collect Rooms")]
    void CollectRooms()
    {
        string[] guids = AssetDatabase.FindAssets("t:EnemyData");

        enemyDatas = new List<EnemyData>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            EnemyData enemyData = AssetDatabase.LoadAssetAtPath<EnemyData>(path);

            if (enemyData != null)
            {
                enemyDatas.Add(enemyData);
            }
        }

        EditorUtility.SetDirty(this);
    }
#endif
}
