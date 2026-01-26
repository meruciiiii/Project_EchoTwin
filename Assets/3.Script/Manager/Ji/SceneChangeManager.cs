using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeManager : MonoBehaviour
{
    public enum SceneType
    {
        Title,
        Lobby,
        Game,
        Ending,
    }

    public static SceneChangeManager instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SceneChange(SceneType name)
    {

    }
}
