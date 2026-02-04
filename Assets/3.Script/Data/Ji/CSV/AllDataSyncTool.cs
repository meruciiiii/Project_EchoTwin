using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
public class AllDataSyncTool
{
    [MenuItem("Tools/Sync All CSV Data")]
    public static void SyncAll()
    {
        Debug.Log("CSV Sync Start");

        WeaponImporter.SyncAndImport();
        MapImporter.SyncAndImport();
        EnemyImporter.SyncAndImport();
        CharacterImport.SyncAndImport();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("CSV Sync Complete");
    }
}
#endif
