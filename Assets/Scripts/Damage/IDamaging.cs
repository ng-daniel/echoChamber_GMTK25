using System;
using UnityEngine;

public interface IDamaging
{
    DamageData GetDamageData();
    public static bool TryHitTarget(GameObject target, DamageData damageData, LayerMask optionalCollisionIgnores)
    {
        if (target == damageData.GetAttacker())
            // avoid hitting source of damage
            return false;

        if (target.layer == damageData.GetAttackerLayer())
            // avoid hitting same layer as source of damage
            return false;

        IDamaging damageInterface = target.GetComponent<IDamaging>();
        if (damageInterface != null &&
            damageInterface.GetDamageData().GetAttackerLayer() == damageData.GetAttackerLayer())
            // if target is also damaging, don't hit if it's from the same source layer
            return false;

        if (optionalCollisionIgnores != 0 &&
            (((1 << target.layer) & optionalCollisionIgnores.value) != 0))
            // is there a layermask override - if so does it apply to the current target?
            return false;

        Health hp = target.GetComponent<Health>();
        if (hp != null)
            hp.Damage(new(damageData));

        return true;
    }
}