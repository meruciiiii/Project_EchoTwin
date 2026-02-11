using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public enum SceneType
    {
        Title,
        Game,
        Die,
    }

    private SceneType currentScene = SceneType.Title;
    public SceneType CurrentScene => currentScene;

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
        if (name == currentScene) return;

        currentScene = name;
        SceneManager.LoadScene((int)name);
    }
}
