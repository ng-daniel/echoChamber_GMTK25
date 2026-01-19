using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using System.Linq.Expressions;

/// <summary>
/// Given a parent object, stores a dict of all immediate children.
/// </summary>
public static class ObjMux
{
    public static void ActOnChildren(GameObject sourceObj, Action<GameObject> action)
    {
        Transform[] children = sourceObj.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            action(child.gameObject);
        }
    }
    static void SelectObject(GameObject obj)
    {
        obj.SetActive(true);
    }
    static void SelectAll(GameObject sourceObj)
    {
        ActOnChildren(sourceObj, SelectObject);
    }
    static void DeSelectObject(GameObject obj)
    {
        obj.SetActive(false);
    }
    static void DeSelectAll(GameObject sourceObj)
    {
        ActOnChildren(sourceObj, DeSelectObject);
    }
    static GameObject GetObject(GameObject obj, string name)
    {
        Transform[] children = obj.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.gameObject.name == name)
            {
                return child.gameObject;
            }
        }
        return null;
    }
    public static void Select(GameObject obj, string name)
    {
        GameObject target = GetObject(obj, name);
        if (target != null && !target.activeSelf)
        {
            target.SetActive(true);
        }
    }
    public static void DeSelect(GameObject obj, string name)
    {
        GameObject target = GetObject(obj, name);
        if (target != null && target.activeSelf)
        {
            target.SetActive(false);
        }
    }
    /// <summary>
    /// Make the target object the only child active
    /// </summary>
    /// <param name="name"></param>
    public static void SelectExclusive(GameObject obj, string name)
    {
        DeSelectAll(obj);
        Select(obj, name);
    }
    /// <summary>
    /// Make the target object the only child not active
    /// </summary>
    /// <param name="name"></param>
    public static void DeSelectExcluive(GameObject obj, string name)
    {
        SelectAll(obj);
        DeSelect(obj, name);
    }
}
