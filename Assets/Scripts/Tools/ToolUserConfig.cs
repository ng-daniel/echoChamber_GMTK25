using UnityEngine;
using System;

namespace Tools
{
    [Serializable]
    public class ToolUserConfig : ScriptableObject
    {
        [SerializeField]
        public LayerMask damageableLayers;
        public LayerMask collideableLayers;
    }
}