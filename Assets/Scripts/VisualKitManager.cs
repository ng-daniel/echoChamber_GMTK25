using Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class VisualKitManager : MonoBehaviour
{
    [Serializable]
    public struct VisualKit
    {
        [SerializeField] public string tag;
        [SerializeField] public GameObject visObj;
    }
    [SerializeField] List<VisualKit> visualKits;
    GameObject currentVisual = null;

    void Awake()
    {
        if (visualKits.Count > 0) currentVisual = visualKits[0].visObj;
    }

    public GameObject GetKit(string tag)
    {
        foreach (VisualKit kit in visualKits)
        {
            if (kit.tag == tag)
            {
                return kit.visObj;
            }
        }
        Debug.LogError("VisualKitManager -> GetKit: No kit found for the given tag");
        return null;
    }
    public void SelectKit(string tag)
    {
        bool selected = false;
        foreach (VisualKit kit in visualKits)
        {
            if (kit.tag.Equals(tag))
            {
                if (selected == true)
                {
                    Debug.LogError("VisualKitManager -> SelectKit: Selected kit twice, indicates possible duplicate tag IDs");
                }

                kit.visObj.SetActive(true);
                currentVisual = kit.visObj;
                selected = true;
            }
            else
            {
                kit.visObj.SetActive(false);
            }
        }

        if (!selected)
        {
            Debug.LogError("VisualKitManager -> SelectKit: No kit found for the given tag");
        }
    }

    public GameObject GetCurrentKit()
    {
        return currentVisual;
    }
}