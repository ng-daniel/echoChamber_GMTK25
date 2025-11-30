using UnityEngine;


public class ESMDataObject : ScriptableObject
{
    [SerializeField] bool isPlayer = false;

    [Header("Movement Params")]
    [SerializeField] float speed;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTimeSeconds;
    [SerializeField] float dashCooldownSeconds;

    [Header("Gun Params")]
    [SerializeField] float bulletSpeed;
    [SerializeField] int bulletDamage;
    [SerializeField] float fireIntervalSeconds;
    [SerializeField] float gunDistanceFromBody;

    [Header("Hurt Params")]
    [SerializeField] float invulnerableDuration;
    [SerializeField] float hitFlashInterval;
    [SerializeField] float deathTimer;
}