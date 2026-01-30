using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public static class CharacterImport
{
    private const string csvPath = "Assets/Data/UpgradeTable.csv";
    private const string outputPath = "Assets/Data/Character";

    [MenuItem("Tools/Sync Character CSV From Google Sheet")]
    public static void SyncAndImport()
    {
        //파일->공유->다른사용자와 공유->링크가 있는 사용자
        //웹에 게시(csv) 후 주소 복사
        string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQSBVM5OLYnSDKKMQonPh0Jopw5olheEDc6xjxfBdEPc1LnQ8zKaHv8GE059V5HHo_AsBqXSvNtlGjm/pub?gid=0&single=true&output=csv";
        string localPath = "Assets/Data/EnemyDataTable.csv";

        ConnectGoogle.DownloaCSV(url, localPath);

        Import();
    }

    //[MenuItem("Tools/Import Character CSV")]
    public static void Import()
    {
        string[][] table = CSVReader.Read(csvPath);

        foreach (var cols in table)
        {
            CharacterRow row = ParseRow(cols);
            CreateOrUpdateCharacter(row);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Character CSV Import Complete");
    }

    private static CharacterRow ParseRow(string[] c)
    {
        return new CharacterRow
        {
            id = ParseInt(c[0]),
            name = c[1],
            targetStat = c[2],
            maxLv = ParseInt(c[3]),
            cost = ParseInt(c[4]),
            costIncrease = ParseFloat(c[5]),
            valuePerLv = ParseFloat(c[6]),
            description = c[7],
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

    private static void CreateOrUpdateCharacter(CharacterRow rows)
    {
        string assetPath = $"{outputPath}/Character_{rows.id}.asset";
        CharacterData data = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<CharacterData>();
            AssetDatabase.CreateAsset(data, assetPath);
        }

        data.id = rows.id;
        data.name = rows.name;
        data.targetStat = rows.targetStat;
        data.maxLv = rows.maxLv;
        data.cost = rows.cost;
        data.costIncrease = rows.costIncrease;
        data.valuePerLv = rows.valuePerLv;
        data.description = rows.description;

        EditorUtility.SetDirty(data);
    }
}
#endif
