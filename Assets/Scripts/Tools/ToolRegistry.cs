using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class ToolRegistry : MonoBehaviour
    {
        [Serializable]
        public class ToolEntry
        {
            public string toolID;
            public GameObject toolPrefab;
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

        public GameObject GetToolPrefabByID(string toolID)
        {
            if (!initialized) Initialize();

            foreach (var entry in toolEntries)
            {
                if (entry.toolID == toolID)
                {
                    return entry.toolPrefab;
                }
            }
            Debug.LogError($"ToolRegistry: Tool with ID {toolID} not found.");
            return null;
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