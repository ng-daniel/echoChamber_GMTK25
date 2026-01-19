using System;
using UnityEngine;

using Tools;
public class WeaponBase : MonoBehaviour, ITool
{
    bool isActive = false;
    SpriteRenderer gunSprite;
    [SerializeField] GameObject bullet;
    DamageData bulletDamageData;
    WeaponBaseStats stats;
    float fireTimer = 0f;
    bool fireReady = false;
    ToolUserConfig userConfig;
    VisualKitManager visKit;

    public string GetToolID()
    {
        return ToolID.WEAPON_BASE;
    }
    public void Initialize(GameObject sourceObj, ToolUserConfig config, ScriptableObject statsObj)
    {
        if (statsObj is WeaponBaseStats)
        {
            this.stats = statsObj as WeaponBaseStats;
        }
        else
        {
            throw new Exception("WeaponBase -> Initialize: incorrect stats object!");
        }

        bulletDamageData = new(sourceObj, stats.bulletDamage);
        userConfig = config;

        visKit = GetComponentInChildren<VisualKitManager>();
        visKit.SelectKit(sourceObj.tag);
        gunSprite = visKit.GetCurrentKit().GetComponent<SpriteRenderer>();
        gunSprite.enabled = false;
    }

    void Update()
    {
        if (!fireReady)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer > stats.fireIntervalSeconds)
            {
                fireTimer = 0;
                SetFireReady(true);
            }
        }
    }
    public void HandleInputs(InputData inputData, ToolUser user)
    {
        if (!isActive) return;

        Vector2 aimDirection = inputData.GetAimDirection();
        ToolUtility.AimToolAutoApply(aimDirection, transform, gunSprite);
        ToolUtility.SetDistFromBody(user.gameObject.transform, transform, aimDirection, stats.gunDistanceFromBody);

        bool mouseHold = inputData.IsMouseHold();
        if (mouseHold && isActive && fireReady)
        {
            FireBullet(aimDirection);
        }
    }
    void FireBullet(Vector2 direction)
    {
        SetFireReady(false);

        BulletFunctionality b = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<BulletFunctionality>();
        b.Initialize(direction.normalized, transform.rotation.eulerAngles.z, stats.bulletSpeed, bulletDamageData, userConfig);
        b.OptionalCollisionIgnores(stats.collisionOverrides);
    }
    void SetFireReady(bool val)
    {
        fireReady = val;
    }

    public void Equip()
    {
        isActive = true;
        gunSprite.enabled = true;
    }
    public void Unequip()
    {
        isActive = false;
        gunSprite.enabled = false;
    }
    public bool GetIsActive()
    {
        return isActive;
    }
}