using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    [Serializable]
    public class ToolUser : MonoBehaviour
    {
        ToolRegistry toolRegistry;
        [SerializeField] List<ToolInfo> toolInventory;
        List<ITool> toolInstances;
        ITool currentTool;
        int currentToolIndex = 0;
        bool toolUserActive;

        void Start()
        {
            toolRegistry = FindFirstObjectByType<ToolRegistry>();
            toolUserActive = false;
            InitializeTools();
        }
        public void Activate()
        {
            toolUserActive = true;
        }
        public void DeActivate()
        {
            toolUserActive = false;
        }
        public void InitializeTools()
        {
            toolInstances = new List<ITool>();
            foreach (var toolInfo in toolInventory)
            {
                GameObject toolPrefab = toolRegistry.GetToolPrefabByID(toolInfo.GetToolID());
                GameObject toolObj = Instantiate(toolPrefab, transform);
                ITool toolInstance = toolObj.GetComponent<ITool>();
                toolInstances.Add(toolInstance);
                toolObj.SetActive(false);
            }
            SwapDirect(0);
        }
        public void SwapScrolling(int direction)
        {
            if (toolInstances.Count == 0) return;

            // deactivate current tool
            if (currentTool != null)
            {
                currentTool.Unequip();
                (currentTool as MonoBehaviour).gameObject.SetActive(false);
            }

            // shift
            currentToolIndex += direction;
            if (currentToolIndex < 0) currentToolIndex = toolInstances.Count - 1; // cyclical swapping
            if (currentToolIndex >= toolInstances.Count) currentToolIndex = 0;

            // activate new tool
            currentTool = toolInstances[currentToolIndex];
            (currentTool as MonoBehaviour).gameObject.SetActive(true);
            currentTool.Equip();
        }

        public void SwapDirect(int index)
        {
            if (toolInstances.Count == 0) return;
            if (index < 0 || index >= toolInstances.Count) return;

            // deactivate current tool
            if (currentTool != null)
            {
                currentTool.Unequip();
                (currentTool as MonoBehaviour).gameObject.SetActive(false);
            }

            // set new tool
            currentToolIndex = index;

            // activate new tool
            currentTool = toolInstances[currentToolIndex];
            (currentTool as MonoBehaviour).gameObject.SetActive(true);
            currentTool.Equip();
        }

        public void HandleInputs(InputData inputData)
        {
            if (toolUserActive && currentTool != null)
                currentTool.HandleInputs(inputData, this);
        }
    }
}
