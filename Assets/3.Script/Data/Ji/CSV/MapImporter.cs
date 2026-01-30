using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public class MapImporter
{
    private const string csvPath = "Assets/Data/MapDataTable.csv";
    private const string outputPath = "Assets/Data/Map";

    [MenuItem("Tools/Sync Map CSV From Google Sheet")]
    public static void SyncAndImport()
    {
        //파일->공유->다른사용자와 공유->링크가 있는 사용자
        //웹에 게시(csv) 후 주소 복사
        string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSdPCDiSs9BQxYe-fw3S2W6p55k0n945WoPVoqwPKIqE0fkgX7x0iTIaZIoCb4RfmWPFdpRAVn7R6LP/pub?gid=0&single=true&output=csv";
        string localPath = "Assets/Data/EnemyDataTable.csv";

        ConnectGoogle.DownloaCSV(url, localPath);

        Import();
    }

    //[MenuItem("Tools/Import Map CSV")]
    public static void Import()
    {
        string[][] table = CSVReader.Read(csvPath);

        foreach (var cols in table)
        {
            MapRow row = ParseRow(cols);
            CreateOrUpdateMap(row);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Map CSV Import Complete");
    }

    private static MapRow ParseRow(string[] c)
    {
        return new MapRow
        {
            ID = ParseInt(c[0]),
            name = c[1],
            floorTilePath = c[2],
            wallTilePaht = c[3],
            poolIDs = ParseIntList(c[4]),
            bossID = ParseInt(c[5]),
            minRoom = ParseInt(c[6]),
            maxRoom = ParseInt(c[7]),
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

    private static void CreateOrUpdateMap(MapRow rows)
    {
        string assetPath = $"{outputPath}/Map_{rows.ID}.asset";
        MapData data = AssetDatabase.LoadAssetAtPath<MapData>(assetPath);

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<MapData>();
            AssetDatabase.CreateAsset(data, assetPath);
        }

        data.id = rows.ID;
        data.mapName = rows.name;
        data.poolIDs = rows.poolIDs;
        data.bossID = rows.bossID;
        data.minRoom = rows.minRoom;
        data.maxRoom = rows.maxRoom;

        data.floorTile = LoadSprite(rows.floorTilePath);
        data.wallTile = LoadSprite(rows.wallTilePaht);

        EditorUtility.SetDirty(data);
    }

    private static List<int> ParseIntList(string s)
    {
        List<int> list = new List<int>();

        if (string.IsNullOrWhiteSpace(s)) return list;

        string[] poolNumb = s.Split(';');

        foreach(string numb in poolNumb)
        {
            string trimmed = numb.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            if(int.TryParse(trimmed,out int value))
            {
                list.Add(value);
            }
        }

        return list;
    }

    private static Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        Sprite Sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        if (Sprite == null)
        {
            Debug.Log("Sprite not founded");
        }

        return Sprite;
    }
}
#endif

