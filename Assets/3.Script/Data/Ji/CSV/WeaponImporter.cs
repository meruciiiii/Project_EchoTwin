using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public static class WeaponImporter
{
    private const string csvPath = "Assets/Data/WeaponDataTable.csv";
    private const string outputPath = "Assets/Data/Weapon";

    [MenuItem("Tools/Sync Weapon CSV From Google Sheet")]
    public static void SyncAndImport()
    {
        //파일->공유->다른사용자와 공유->링크가 있는 사용자
        //웹에 게시(csv) 후 주소 복사
        string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQogVKqcICNFgPz5xQ4Qn8LkrB0jiASyYzdlwybkagRZhIRF_5gH6GBPyPg-V79TgY6mqBd2eW2-u9R/pub?gid=0&single=true&output=csv";
        string localPath = "Assets/Data/EnemyDataTable.csv";
    
        ConnectGoogle.DownloaCSV(url, localPath);
    
        Import();
    }

    //[MenuItem("Tools/Import Weapon CSV")]
    public static void Import()
    {
        string[][] table = CSVReader.Read(csvPath);

        foreach (var cols in table)
        {
            WeaponRow row = ParseRow(cols);
            CreateOrUpdateWeapon(row);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Weapon CSV Import Complete");
    }

    private static WeaponRow ParseRow(string[] c)
    {
        return new WeaponRow
        {
            ID = ParseInt(c[0]),
            name = c[1],
            identity = c[2],
            type = c[3],
            baseDamage = ParseFloat(c[4]),
            attackSpeed = ParseFloat(c[5]),
            attackRange = ParseFloat(c[6]),
            knockback = ParseFloat(c[7]),
            echoDMGRatio = ParseFloat(c[8]),
            echoDescription = c[9],
            iconPath = c[10]
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

    private static void CreateOrUpdateWeapon(WeaponRow rows)
    {
        string assetPath = $"{outputPath}/Weapon_{rows.ID}.asset";
        WeaponData data = AssetDatabase.LoadAssetAtPath<WeaponData>(assetPath);

        if(data == null)
        {
            data = ScriptableObject.CreateInstance<WeaponData>();
            AssetDatabase.CreateAsset(data, assetPath);
        }

        data.id = rows.ID;
        data.weaponName = rows.name;
        data.identity = rows.identity;
        data.type = rows.type;

        data.baseDamage = rows.baseDamage;
        data.attackSpeed = rows.attackSpeed;
        data.attackRange = rows.attackRange;
        data.knockback = rows.knockback;

        data.echoDMGRatio = rows.echoDMGRatio;
        data.echoDescription = rows.echoDescription;

        data.icon = LoadSprite(rows.iconPath);

        EditorUtility.SetDirty(data);
    }

    private static Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        if(sprite == null)
        {
            Debug.Log("sprite not founded");
        }

        return sprite;
    }
}
#endif
