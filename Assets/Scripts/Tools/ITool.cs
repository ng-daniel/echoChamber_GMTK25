using System;
using UnityEditor;
using UnityEngine;

using InputDataSystem;

namespace Tools
{
    public interface ITool
    {
        public void HandleInputs(InputData inputData, ToolUser toolUser);
        public bool GetIsActive();
        public void Equip();
        public void Unequip();
        public string GetToolID();
        public void Initialize(GameObject toolUserObject, ToolUserConfig config, ScriptableObject stats);
    }
}
