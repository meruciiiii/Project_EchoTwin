using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class CSVReader
{
    public static string[][] Read(string path)
    {
        var line = File.ReadAllLines(path);
        var table = new string[line.Length - 1][];

        for(int i=1;i<line.Length;i++)
        {
            table[i - 1] = line[i].Split(',');
        }

        return table;
    }
}
