using Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace VisualKits
{
    [Serializable]
    public struct VisKitSubObject
    {
        [SerializeField] public string id;
        [SerializeField] public GameObject obj;
    }

    [Serializable]
    public struct VisualKit
    {
        [SerializeField] public string tag;
        [SerializeField] public GameObject visObj;
        [SerializeField] public List<VisKitSubObject> subObjects;

        public VisualKit(string tag, GameObject visObj, List<VisKitSubObject> subObjects)
        {
            this.tag = tag;
            this.visObj = visObj;
            this.subObjects = subObjects;
        }

        public GameObject GetSubObjectByID(string id)
        {
            foreach (VisKitSubObject subObj in subObjects)
            {
                if (subObj.id == id)
                {
                    return subObj.obj;
                }
            }
            Debug.LogError("VisualKit -> GetSubObjectByID: No sub-object found for the given ID");
            return null;
        }
    }

    public class VisualKitManager : MonoBehaviour
    {

        [SerializeField] List<VisualKit> visualKits;
        VisualKit currentVisual;

        void Awake()
        {
            if (visualKits.Count > 0) currentVisual = visualKits[0];
        }

        public VisualKit GetKit(string tag)
        {
            foreach (VisualKit kit in visualKits)
            {
                if (kit.tag == tag)
                {
                    return kit;
                }
            }
            Debug.LogError("VisualKitManager -> GetKit: No kit found for the given tag");
            return new("ERROR", null, null);
        }
        public VisualKit SelectKit(string tag)
        {
            bool selected = false;
            VisualKit selectedKit = new();
            foreach (VisualKit kit in visualKits)
            {
                if (kit.tag.Equals(tag))
                {
                    if (selected == true)
                    {
                        Debug.LogError("VisualKitManager -> SelectKit: Selected kit twice, indicates possible duplicate tag IDs");
                    }

                    kit.visObj.SetActive(true);
                    currentVisual = kit;
                    selectedKit = kit;
                    selected = true;
                }
                else
                {
                    kit.visObj.SetActive(false);
                }
            }

            if (selected)
            {
                return selectedKit;
            }
            Debug.LogError("VisualKitManager -> SelectKit: No kit found for the given tag");
            return new("ERROR", null, null);
        }

        public VisualKit GetCurrentKit()
        {
            return currentVisual;
        }
    }
}
