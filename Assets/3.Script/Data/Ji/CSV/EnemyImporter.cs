using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public class EnemyImporter
{
    private const string csvPath = "Assets/Data/EnemyDataTable.csv";
    private const string outputPath = "Assets/Data/Enemy";

    [MenuItem("Tools/Sync Enemy CSV From Google Sheet")]
    public static void SyncAndImport()
    {
        //파일->공유->다른사용자와 공유->링크가 있는 사용자
        //웹에 게시(csv) 후 주소 복사
        string url = "https://docs.google.com/spreadsheets/d/XXXX/export?format=csv";
        string localPath = "Assets/Data/EnemyDataTable.csv";
    
        ConnectGoogle.DownloaCSV(url, localPath);
    
        Import();
    }

    //[MenuItem("Tools/Import Enemy CSV")]
    public static void Import()
    {
        string[][] table = CSVReader.Read(csvPath);

        foreach (var cols in table)
        {
            EnemyRow row = ParseRow(cols);
            CreateOrUpdateEnemy(row);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Enemy CSV Import Complete");
    }

    private static EnemyRow ParseRow(string[] c)
    {
        return new EnemyRow
        {
            id = ParseInt(c[0]),
            name = c[1],
            tier = c[2],
            maxHP = ParseInt(c[3]),
            damage = ParseInt(c[4]),
            moveSpeed = ParseFloat(c[5]),
            detectRange = ParseFloat(c[6]),
            attackRange = ParseFloat(c[7]),
            coolTime = ParseFloat(c[8]),
            dropEXP = ParseInt(c[9]),
            dropGold = ParseInt(c[10]),
            prefabPath = c[11]
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

    private static void CreateOrUpdateEnemy(EnemyRow rows)
    {
        string assetPath = $"{outputPath}/Enemy_{rows.id}.asset";
        EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>(assetPath);

        if (data == null)
        {
            data = ScriptableObject.CreateInstance<EnemyData>();
            AssetDatabase.CreateAsset(data, assetPath);
        }

        data.id = rows.id;
        data.name = rows.name;
        data.tier = rows.tier;
        data.maxHP = rows.maxHP;
        data.damage = rows.damage;
        data.moveSpeed = rows.moveSpeed;
        data.detectRange = rows.detectRange;
        data.attackRange = rows.attackRange;
        data.coolTime = rows.coolTime;
        data.dropEXP = rows.dropEXP;
        data.dropGold = rows.dropGold;

        data.prefab = LoadPrefab(rows.prefabPath);

        EditorUtility.SetDirty(data);
    }

    private static GameObject LoadPrefab(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (prefab == null)
        {
            Debug.Log("prefab not founded");
        }

        return prefab;
    }
}
#endif

