using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public static class UpgradeTableLoader
{
    private const string csvPath = "Assets/Data/UpgradeTable.csv";
    public static Dictionary<int, UpgradeData> Load()
    {
        string[][] table = CSVReader.Read(csvPath);
        Dictionary<int, UpgradeData> dict = new Dictionary<int, UpgradeData>();

        foreach (var cols in table)
        {
            UpgradeData data = new UpgradeData
            {
                id = ParseInt(cols[0]),
                name = cols[1],
                targetStat = cols[2],
                maxLv = ParseInt(cols[3]),
                baseCost = ParseInt(cols[4]),
                costIncrease = ParseFloat(cols[5]),
                valuePerLv = ParseFloat(cols[6]),
                description = cols[7],
            };

            dict[data.id] = data;
        }

        return dict;
    }

    static int ParseInt(string v) => string.IsNullOrEmpty(v) ? 0 : int.Parse(v);
    static float ParseFloat(string v) => string.IsNullOrEmpty(v) ? 0f : float.Parse(v);
}
#endif