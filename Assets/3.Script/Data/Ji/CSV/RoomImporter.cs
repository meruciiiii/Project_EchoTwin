using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public static class RoomImporter
{
    private const string csvPath = "Assets/Data/RoomDataTable.csv";
    private const string outputPath = "Assets/Data/Room";

    [MenuItem("Tools/Sync Room CSV From Google Sheet")]
    public static void SyncAndImport()
    {
        //파일->공유->다른사용자와 공유->링크가 있는 사용자
        //웹에 게시(csv) 후 주소 복사
        string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vT_3Ytu5rp1kALFa1k7VaOahrC_kx543xOOdMBjxCtS-gaPlq0lq0bncoVjOI7U7CY0FsCvjNrDKmzN/pub?gid=0&single=true&output=csv";
        string localPath = "Assets/Data/RoomDataTable.csv";

        ConnectGoogle.DownloaCSV(url, localPath);

        Import();
    }

    public static void Import()
    {
        string[][] table = CSVReader.Read(csvPath);

        foreach (var cols in table)
        {
            RoomRow row = ParseRow(cols);
            CreateOrUpdateRoom(row);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Room CSV Import Complete");
    }

    private static RoomRow ParseRow(string[] c)
    {
        return new RoomRow
        {
            id = ParseInt(c[0]),
            roomType = c[1],
            monsterPackID = ParseInt(c[2])
        };
    }

    private static float ParseFloat(string value)
    {
        return string.IsNullOrEmpty(value) ? 0f : float.Parse(value);
    }

    private static int ParseInt(string value)
    {
        return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    private static void CreateOrUpdateRoom(RoomRow rows)
    {
        string assetPath = $"{outputPath}/Room_{rows.id}.asset";
        RoomData data = AssetDatabase.LoadAssetAtPath<RoomData>(assetPath);

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<RoomData>();
            AssetDatabase.CreateAsset(data, assetPath);
        }

        data.id = rows.id;
        data.roomType = rows.roomType;
        data.monsterPackID = rows.monsterPackID;

        EditorUtility.SetDirty(data);
    }

    private static Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        if (sprite == null)
        {
            Debug.Log("sprite not founded");
        }

        return sprite;
    }
}
#endif
