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
        [SerializeField] List<ITool> toolInstances;
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
            if (currentTool != null)
                currentTool.Equip();
            else
                SwapDirect(0);
        }
        public void DeActivate()
        {
            toolUserActive = false;
            if (currentTool != null) currentTool.Unequip();
        }
        public void InitializeTools()
        {
            toolInstances = new List<ITool>();
            foreach (var toolInfo in toolInventory)
            {
                print("Initializing tool: " + toolInfo.GetToolID());
                GameObject toolPrefab = toolRegistry.GetToolPrefabByID(toolInfo.GetToolID());
                GameObject toolObj = Instantiate(toolPrefab, transform);
                ITool toolInstance = toolObj.GetComponent<ITool>();
                toolInstances.Add(toolInstance);
                toolInstance.Unequip();
            }
        }
        public void SwapScrolling(int direction)
        {
            if (toolInstances.Count == 0) return;

            // deactivate current tool
            if (currentTool != null)
            {
                currentTool.Unequip();
            }

            // shift
            currentToolIndex += direction;
            if (currentToolIndex < 0) currentToolIndex = toolInstances.Count - 1; // cyclical swapping
            if (currentToolIndex >= toolInstances.Count) currentToolIndex = 0;

            // activate new tool
            currentTool = toolInstances[currentToolIndex];
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
            }

            // set new tool
            currentToolIndex = index;

            // activate new tool
            currentTool = toolInstances[currentToolIndex];
            currentTool.Equip();
        }

        public void HandleInputs(InputData inputData)
        {
            if (toolUserActive && currentTool != null)
                currentTool.HandleInputs(inputData, this);
        }
    }
}
