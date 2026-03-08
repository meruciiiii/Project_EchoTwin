using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UpgradeTableLoader
{
    private const string csvPath = "Data/UpgradeTable.csv";
    public static Dictionary<int, UpgradeData> Load()
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, csvPath);
        Dictionary<int, UpgradeData> dict = new Dictionary<int, UpgradeData>();

        if (!File.Exists(fullPath))
        {
            Debug.LogError("UpgradeTable.csv not found: " + fullPath);
            return dict; // [CHANGED]
        }

        string[][] table = CSVReader.Read(fullPath);

        foreach (var cols in table)
        {
            if (cols == null || cols.Length < 8)
                continue;

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

    static int ParseInt(string v)
    {
        if (string.IsNullOrEmpty(v)) return 0;
        int.TryParse(v, out int result);
        return result;
    }

    static float ParseFloat(string v)
    {
        if (string.IsNullOrEmpty(v)) return 0f;
        float.TryParse(
            v,
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out float result
        );
        return result;
    }
}
