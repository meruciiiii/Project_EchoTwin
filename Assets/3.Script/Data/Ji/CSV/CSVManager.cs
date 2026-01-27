using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class RuntimeCSVConfig
{
    public string key;
    public string fileName;
    public string url;
}

public class CSVManager : MonoBehaviour
{
    public List<RuntimeCSVConfig> csvList = new List<RuntimeCSVConfig>();

    private Dictionary<string, string[]> loadedCSVs = new();

    private void Start()
    {
        StartCoroutine(LoadAll());
    }

    private IEnumerator LoadAll()
    {
        foreach(var csv in csvList)
        {
            yield return EnsureCSV(csv);
        }

        Debug.Log("All SCV loaded");
    }

    private IEnumerator EnsureCSV(RuntimeCSVConfig config)
    {
        string path = Path.Combine(Application.persistentDataPath, config.fileName);

        if(!File.Exists(path))
        {
            yield return null;
        }

        loadedCSVs[config.key] = File.ReadAllLines(path);
    }
}
