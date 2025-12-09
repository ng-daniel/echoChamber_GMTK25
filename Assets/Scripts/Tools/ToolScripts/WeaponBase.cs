using System;
using UnityEngine;

using Tools;
public class WeaponBase : MonoBehaviour, ITool
{
    bool isActive = false;
    SpriteRenderer gunSprite;
    [SerializeField] GameObject bullet;
    DamageData bulletDamageData;
    [SerializeField] float bulletSpeed;
    [SerializeField] int bulletDamage;
    [SerializeField] float fireIntervalSeconds;
    [SerializeField] float gunDistanceFromBody;
    float fireTimer = 0f;
    bool fireReady = false;

    public string GetToolID()
    {
        return ToolID.WEAPON_BASE;
    }
    void Awake()
    {
        gunSprite = GetComponent<SpriteRenderer>();
        gunSprite.enabled = false;
        bulletDamageData = new(this.gameObject, bulletDamage);
        print("DONE LOADING WEAPON BASE");
    }

    void Update()
    {
        if (!fireReady)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer > fireIntervalSeconds)
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
        ToolUtility.SetDistFromBody(user.gameObject.transform, transform, aimDirection, gunDistanceFromBody);

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
        print(bulletDamageData.GetDamage());
        b.Initialize(direction.normalized, transform.rotation.eulerAngles.z, bulletSpeed, bulletDamageData);
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