#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Map/FloorData")]
public class FloorScriptableObject : ScriptableObject
{
    public List<RoomData> rooms;
#if UNITY_EDITOR
    [ContextMenu("Auto Collect Rooms")]
    void CollectRooms()
    {
        string[] guids = AssetDatabase.FindAssets("t:RoomData");

        rooms = new List<RoomData>();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            RoomData room = AssetDatabase.LoadAssetAtPath<RoomData>(path);

            if (room != null)
            {
                rooms.Add(room);
            }
        }

        EditorUtility.SetDirty(this);
    }
#endif
}
