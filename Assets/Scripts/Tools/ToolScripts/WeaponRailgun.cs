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
    RailgunState currentState = RailgunState.READY;
    bool isActive = false;
    SpriteRenderer gunSprite;
    [SerializeField] GameObject piercerPrefab;
    PiercerFunctionality piercerInstance;
    DamageData laserDamageData;
    WeaponRailgunStats stats;
    ToolUserConfig userConfig;
    VisualKitManager visKit;
    Vector2 inputAimDir;
    float chargeTimer = 0f;
    float cooldownTimer = 0f;

    public string GetToolID()
    {
        return ToolID.WEAPON_RAILGUN;
    }

    public void HandleInputs(InputData inputData, ToolUser toolUser)
    {
        if (!isActive) return;

        Vector2 aimDirection = inputData.GetAimDirection();
        ToolUtility.AimToolAutoApply(aimDirection, transform, gunSprite);
        ToolUtility.SetDistFromBody(toolUser.gameObject.transform, transform, aimDirection, stats.gunDistanceFromBody);
        inputAimDir = aimDirection;

        bool mouseClick = inputData.IsMouseClick();
        if (mouseClick && isActive && currentState == RailgunState.READY)
        {
            StartCharge();
        }
    }
    void StartCharge()
    {
        chargeTimer = stats.chargeTime;
        piercerInstance = Instantiate(piercerPrefab, this.gameObject.transform).GetComponent<PiercerFunctionality>();
        piercerInstance.Initialize(inputAimDir, laserDamageData, stats.railRadius);
        currentState = RailgunState.CHARGING;
    }
    void FireShot()
    {
        // fire shot logic
        if (piercerInstance != null)
        {
            print("MYINST: " + piercerInstance);
            piercerInstance.SetModeFire(inputAimDir);
        }


        cooldownTimer = stats.fireCooldownSec;
        currentState = RailgunState.COOLDOWN;
    }
    void Reset()
    {
        chargeTimer = 0f;
        cooldownTimer = 0f;
        currentState = RailgunState.READY;
    }
    void Update()
    {
        if (currentState == RailgunState.CHARGING)
        {
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0)
            {
                FireShot();
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

        laserDamageData = new(sourceObj, stats.railDamage);
        userConfig = config;

        visKit = GetComponentInChildren<VisualKitManager>();
        VisualKit myKit = visKit.SelectKit(sourceObj.tag);


        gunSprite = visKit.GetCurrentKit().visObj.GetComponent<SpriteRenderer>();
        gunSprite.enabled = false;
    }
    public void Equip()
    {
        isActive = true;
        gunSprite.enabled = true;
    }
    public void Unequip()
    {
        if (currentState == RailgunState.CHARGING)
        {
            piercerInstance.SetModeOff();
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