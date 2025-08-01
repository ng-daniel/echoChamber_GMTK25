using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    int healthValue;
    [SerializeField] int maxHealth;
    [SerializeField] LayerMask predatorLayers;

    public delegate DamageData DamageFilter(DamageData damageData);
    List<DamageFilter> damageFilterList = new List<DamageFilter>();
    public delegate void DeathEvent();
    DeathEvent OnDeath;


    void Start()
    {
        healthValue = maxHealth;
        OnDeath = DefaultDeathEvent;
    }

    public void SetHealth(int val)
    {
        healthValue = val;
    }
    public int GetHealth()
    {
        return healthValue;
    }
    public void Damage(DamageData damageData)
    {
        if (!damageData.AttackerInLayer(predatorLayers)) return;

        damageData = DamageFilterChain(damageData);
        healthValue -= damageData.GetDamage();
        if (healthValue <= 0)
        {
            OnDeath();
        }
    }
    DamageData DamageFilterChain(DamageData damageData)
    {
        foreach (DamageFilter filter in damageFilterList) damageData = filter(damageData);
        return damageData;
    }
    public void SetDeathEvent(DeathEvent deathEvent)
    {
        OnDeath = deathEvent;
    }
    public void DefaultDeathEvent()
    {
        Destroy(gameObject);
    }
}
