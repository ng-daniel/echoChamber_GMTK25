using UnityEngine;
using Tools;
using System;

[CreateAssetMenu(fileName = "WeaponBaseStats", menuName = "ToolStats/WeaponBase")]
[Serializable]
public class WeaponBaseStats : ScriptableObject
{
    [SerializeField] public float bulletSpeed;
    [SerializeField] public int bulletDamage;
    [SerializeField] public float fireIntervalSeconds;
    [SerializeField] public float gunDistanceFromBody;
    [SerializeField] public LayerMask collisionOverrides = 0;
}