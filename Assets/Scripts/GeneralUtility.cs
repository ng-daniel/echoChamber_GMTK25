using System;
using UnityEngine;

public static class GeneralUtility
{
    public static bool LayerCompare(int layerInt, int maskValue)
    {
        return ((1 << layerInt) & maskValue) != 0;
    }
}