using UnityEngine;
using Tools;
using System;

[CreateAssetMenu(fileName = "WeaponRailgunStats", menuName = "ToolStats/WeaponRailgun")]
[Serializable]
public class WeaponRailgunStats : ScriptableObject
{
    [SerializeField] public float chargeTime;
    [SerializeField] public int railDamage;
    [SerializeField] public float fireCooldownSec;
    [SerializeField] public float gunDistanceFromBody;
    [SerializeField] public LayerMask collisionOverrides = 0;
}