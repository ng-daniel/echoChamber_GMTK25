using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData
{

    GameObject attacker;
    int damageVal;
    int multiplier = 1;
    String context;

    public DamageData(GameObject attacker, int damageVal)
    {
        this.attacker = attacker;
        this.damageVal = damageVal;
    }

    public DamageData(GameObject attacker, int damageVal, string context)
    {
        this.attacker = attacker;
        this.damageVal = damageVal;
        this.context = context;
    }

    public bool AttackerInLayer(LayerMask targetLayer)
    {
        if (attacker == null) return false;
        return 0 != (targetLayer & (1 << attacker.layer));
    }
    public bool AttackerHasTag(String tag)
    {
        return attacker.CompareTag(tag);
    }
    public GameObject GetAttacker()
    {
        return attacker;
    }
    public int GetDamage()
    {
        return damageVal * multiplier;
    }
    public void SetDamage(int val)
    {
        damageVal = val;
    }
    public int GetDamageRaw()
    {
        return damageVal;
    }
    public int GetMultiplier()
    {
        return multiplier;
    }
    public String GetContext()
    {
        return context;
    }
    public void SetContext(String context)
    {
        this.context = context;
    }

}
