using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public class MonsterPackImporter
{
    private const string csvPath = "Assets/Data/MonsterPackDataTable.csv";
    private const string outputPath = "Assets/Data/MonsterPack";

    [MenuItem("Tools/Sync MonsterPack CSV From Google Sheet")]
    public static void SyncAndImport()
    {
        //파일->공유->다른사용자와 공유->링크가 있는 사용자
        //웹에 게시(csv) 후 주소 복사
        string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vS28Ii9KXSvDielSJnUi41jub4XMRecewU8KoQKCB6Q43lcqlnOD59m-iqn5in70LBQzjuolXIDWlUG/pub?gid=0&single=true&output=csv";
        string localPath = "Assets/Data/MonsterPackDataTable.csv";

        ConnectGoogle.DownloaCSV(url, localPath);

        Import();
    }
    public static void Import()
    {
        string[][] table = CSVReader.Read(csvPath);

        foreach (var cols in table)
        {
            MonsterPackRow row = ParseRow(cols);
            CreateOrUpdateMonsterPack(row);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("MonsterPack CSV Import Complete");
    }

    private static MonsterPackRow ParseRow(string[] c)
    {
        return new MonsterPackRow
        {
            monsterPackID = ParseInt(c[0]),
            monsters = ParseMonstertList(c[1]),
            totalCount = ParseInt(c[2]),
            description = c[3]
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

    private static void CreateOrUpdateMonsterPack(MonsterPackRow rows)
    {
        string assetPath = $"{outputPath}/MonsterPack_{rows.monsterPackID}.asset";
        MonsterPackData data = AssetDatabase.LoadAssetAtPath<MonsterPackData>(assetPath);

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<MonsterPackData>();
            AssetDatabase.CreateAsset(data, assetPath);
        }

        data.monsterPackID = rows.monsterPackID;
        data.monsters = rows.monsters;
        data.totalCount = rows.totalCount;
        data.description = rows.description;

        EditorUtility.SetDirty(data);
    }

    private static List<MonsterEntry> ParseMonstertList(string s)
    {
        List<MonsterEntry> list = new List<MonsterEntry>();

        if (string.IsNullOrWhiteSpace(s)) return list;

        foreach (var token in s.Split(';'))
        {
            int open = token.IndexOf('(');
            int close = token.IndexOf(')');

            if (open < 0 || close < 0) continue;

            int id = int.Parse(token.Substring(0, open));
            int count = int.Parse(token.Substring(open + 1, close - open - 1));

            list.Add(new MonsterEntry{monsterID = id,count = count});
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
