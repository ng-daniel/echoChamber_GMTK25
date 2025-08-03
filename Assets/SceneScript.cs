using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour
{

    private static SceneScript instance;
    private SceneScript() { }
    public static SceneScript GetInstance()
    {
        return instance;
    }
    bool changingScenes;
    void Start()
    {
        instance = this;
        if (SceneManager.GetActiveScene().name == "WinScene" || SceneManager.GetActiveScene().name == "LoseScene")
        {
            SetSceneAfterTime("Arena", 5);
        }
    }

    public void SetSceneAfterTime(String name, float seconds)
    {
        if (changingScenes) return;
        StartCoroutine(SetSceneCoroutine(name, seconds));
    }
    IEnumerator SetSceneCoroutine(String name, float seconds)
    {
        changingScenes = true;
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(name);
    }

}
