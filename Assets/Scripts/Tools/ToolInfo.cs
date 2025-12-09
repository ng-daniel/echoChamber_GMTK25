using System;
using UnityEngine;

namespace Tools
{
    [Serializable]
    public class ToolInfo
    {
        [SerializeField] string toolID;
        [SerializeField] bool isUnlocked; // CURRENTLY NON FUNCTIONAL LMAOO
        public ToolInfo(string toolID)
        {
            this.toolID = toolID;
            isUnlocked = false;
        }

        public string GetToolID() { return toolID; }
        public bool GetIsUnlocked() { return isUnlocked; }
        public void SetIsUnlocked(bool val) { isUnlocked = val; }
    }
}