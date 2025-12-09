using System;
using UnityEngine;

namespace Tools
{
    public interface ITool
    {
        public void HandleInputs(InputData inputData, ToolUser toolUser);
        public bool GetIsActive();
        public void Equip();
        public void Unequip();
        public string GetToolID();
    }
}
