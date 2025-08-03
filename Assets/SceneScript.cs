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
        StartCoroutine(SetSceneCoroutine(name, seconds));
    }
    IEnumerator SetSceneCoroutine(String name, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(name);
    }

}
