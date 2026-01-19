using System;
using UnityEngine;

namespace Tools
{
    [Serializable]
    public class ToolInfo
    {
        [SerializeField] string toolID;
        public ToolInfo(string toolID)
        {
            this.toolID = toolID;
        }

        public string GetToolID() { return toolID; }
    }
}