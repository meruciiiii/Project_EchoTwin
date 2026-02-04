using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ConnectGoogle
{
    public static void DownloaCSV(string url, string savePath)
    {
        using (WebClient client = new WebClient())
        {
            client.DownloadFile(url, savePath);
        }
    }
}
