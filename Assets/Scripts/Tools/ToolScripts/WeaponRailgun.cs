using System;
using UnityEngine;

using Tools;
using VisualKits;

public class WeaponRailgun : MonoBehaviour, ITool
{
    public enum RailgunState
    {
        READY,
        CHARGING,
        COOLDOWN
    }
    [SerializeField] RailgunState currentState = RailgunState.READY;
    bool isActive = false;
    SpriteRenderer gunSprite;
    [SerializeField] GameObject piercerPrefab;
    [SerializeField] ParticleSystem chargeEffect;
    [SerializeField] ParticleSystem cooldownEffect;
    DamageData railDamageData;
    WeaponRailgunStats stats;
    ToolUserConfig userConfig;
    VisualKitManager visKit;
    float chargeTimer = 0f;
    float cooldownTimer = 0f;
    bool fireFlag;

    public string GetToolID()
    {
        return ToolID.WEAPON_RAILGUN;
    }

    public void HandleInputs(InputData inputData, ToolUser toolUser)
    {
        if (!isActive) return;

        Vector2 aimDirection = inputData.GetAimDirection();
        if (aimDirection == null)
        {
            print("WeaponRailgun -> HandleInputs: aimDirection is null!");
        }
        ToolUtility.AimToolAutoApply(aimDirection, transform, gunSprite);
        ToolUtility.SetDistFromBody(toolUser.gameObject.transform, transform, aimDirection, stats.gunDistanceFromBody);

        bool mouseHold = inputData.IsMouseHold();
        if (mouseHold && isActive && currentState == RailgunState.READY)
        {
            StartCharge();
        }
        else if (!mouseHold && currentState == RailgunState.CHARGING)
        {
            Reset();
        }
        if (GetFireFlag())
        {
            ResetFireFlag();
            FireShot(aimDirection);
        }
    }
    void StartCharge()
    {
        chargeTimer = stats.chargeTime;
        chargeEffect.Play();
        currentState = RailgunState.CHARGING;
        print("RAIL CHARGE STARTED");
    }
    void FireShot(Vector2 direction)
    {
        chargeEffect.Stop();
        PiercerFunctionality piercerInstance = Instantiate(piercerPrefab, transform.position, Quaternion.identity).GetComponent<PiercerFunctionality>();
        piercerInstance.Initialize(
            direction,
            transform.rotation.eulerAngles.z,
            stats,
            railDamageData,
            userConfig);
        cooldownTimer = stats.fireCooldownSec;
        currentState = RailgunState.COOLDOWN;
        cooldownEffect.Play();
    }
    void Reset()
    {
        chargeTimer = stats.chargeTime;
        cooldownTimer = stats.fireCooldownSec;
        currentState = RailgunState.READY;
        cooldownEffect.Stop();
        chargeEffect.Stop();
    }
    void Update()
    {
        if (currentState == RailgunState.CHARGING)
        {
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0)
            {
                SetFireFlag();
            }
        }
        if (currentState == RailgunState.COOLDOWN)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                Reset();
            }
        }
    }
    public void Initialize(GameObject sourceObj, ToolUserConfig config, ScriptableObject statsObj)
    {
        if (statsObj is WeaponRailgunStats)
        {
            this.stats = statsObj as WeaponRailgunStats;
        }
        else
        {
            throw new Exception("WeaponRailgun -> Initialize: incorrect stats object!");
        }

        railDamageData = new(sourceObj, stats.railDamage);
        userConfig = config;

        visKit = GetComponentInChildren<VisualKitManager>();
        VisualKit myKit = visKit.SelectKit(sourceObj.tag);

        gunSprite = visKit.GetCurrentKit().visObj.GetComponent<SpriteRenderer>();
        gunSprite.enabled = false;

        cooldownEffect.Stop();
    }
    void SetFireFlag()
    {
        fireFlag = true;
    }
    void ResetFireFlag()
    {
        fireFlag = false;
    }
    bool GetFireFlag()
    {
        return fireFlag;
    }
    public void Equip()
    {
        isActive = true;
        gunSprite.enabled = true;
        if (currentState == RailgunState.COOLDOWN)
        {
            cooldownEffect.Play();
        }
    }
    public void Unequip()
    {
        chargeEffect.Stop();
        cooldownEffect.Stop();
        if (currentState == RailgunState.CHARGING)
        {
            currentState = RailgunState.READY;
        }
        isActive = false;
        gunSprite.enabled = false;
    }
    public bool GetIsActive()
    {
        return isActive;
    }
}