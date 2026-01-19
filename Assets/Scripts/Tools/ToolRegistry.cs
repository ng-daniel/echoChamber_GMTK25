using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public struct ToolQueryResult
    {
        public ToolQueryResult(GameObject prefab, ScriptableObject stats)
        {
            this.prefab = prefab;
            this.stats = stats;
        }
        public GameObject prefab;
        public ScriptableObject stats;
    }
    public class ToolRegistry : MonoBehaviour
    {
        [Serializable]
        public class ToolStatKit
        {
            public string tag;
            public ScriptableObject stats;
        }
        [Serializable]
        public class ToolEntry
        {
            public string toolID;
            public GameObject toolPrefab;
            public List<ToolStatKit> kits;
            public ScriptableObject GetKitForTag(string tag)
            {
                foreach (ToolStatKit kit in kits)
                {
                    if (kit.tag.Equals(tag))
                    {
                        return kit.stats;
                    }
                }
                return null;
            }
        }
        public List<ToolEntry> toolEntries;
        bool initialized = false;

        void Initialize()
        {
            foreach (var entry in toolEntries)
            {
                if (entry.toolPrefab == null)
                {
                    Debug.LogError("ToolRegistry: Tool prefab is null for one of the entries.");
                    continue;
                }
                entry.toolID = entry.toolPrefab.GetComponent<ITool>().GetToolID();
            }
            initialized = true;
        }
        public ToolQueryResult GetToolByID(string toolID, string tag)
        {
            if (!initialized) Initialize();

            foreach (var entry in toolEntries)
            {
                if (entry.toolID == toolID)
                {
                    ScriptableObject targetKit = entry.GetKitForTag(tag);
                    return new ToolQueryResult(entry.toolPrefab, targetKit);
                }
            }
            Debug.LogError($"ToolRegistry: Tool with ID {toolID} not found.");
            return new ToolQueryResult(null, null);
        }

        public string GetToolIDByPrefab(GameObject toolPrefab)
        {
            if (!initialized) Initialize();

            foreach (var entry in toolEntries)
            {
                if (entry.toolPrefab == toolPrefab)
                {
                    return entry.toolID;
                }
            }
            Debug.LogError($"ToolRegistry: Tool with prefab {toolPrefab.name} not found.");
            return null;
        }

    }
}