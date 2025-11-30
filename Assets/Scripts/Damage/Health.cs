using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    [SerializeField] public int healthValue { get; private set; }
    [SerializeField] int maxHealth;
    [SerializeField] LayerMask predatorLayers;
    SpriteRenderer spriteRenderer;
    public delegate DamageData DamageFilter(DamageData damageData);
    public List<DamageFilter> damageFilterList = new List<DamageFilter>();
    public delegate void HitEvent();
    HitEvent OnHit;
    public delegate void DeathEvent();
    DeathEvent OnDeath;
    const float hitFlashInterval = 0.07f;

    void Awake()
    {
        healthValue = maxHealth;
        OnDeath ??= DefaultDeathEvent;
        OnHit ??= DefaultHitEvent;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetHealth(int val)
    {
        healthValue = val;
    }
    public int GetHealth()
    {
        return healthValue;
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public void Damage(DamageData damageData)
    {
        damageData = new(damageData);
        if (!damageData.AttackerInLayer(predatorLayers)) return;

        damageData = DamageFilterChain(damageData);
        if (damageData.GetDamage() == 0)
        {
            return;
        }

        if (healthValue <= 0)
        {
            healthValue = 0;
            return;
        }

        healthValue -= damageData.GetDamage();
        TriggerHitFlash();
        OnHit();
        if (healthValue <= 0)
        {
            OnDeath();
        }
    }
    public void KillNoRegard()
    {
        // kills this entity no matter what
        OnDeath();
    }
    DamageData DamageFilterChain(DamageData damageData)
    {
        foreach (DamageFilter filter in damageFilterList) damageData = filter(damageData);
        return damageData;
    }
    public void ClearFilterList()
    {
        damageFilterList = new List<DamageFilter>();
    }

    public void SetHitEvent(HitEvent hitEvent)
    {
        OnHit = hitEvent;
    }
    public void DefaultHitEvent()
    {
        print(gameObject.name + " says, \"YEEOOOOOWCH!!!\"");
    }
    public void SetDeathEvent(DeathEvent deathEvent)
    {
        OnDeath = deathEvent;
    }
    public void DefaultDeathEvent()
    {
        GlobalEventHolder.OnDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }
    public void TriggerHitFlash()
    {
        StartCoroutine(HitFlash());
    }
    IEnumerator HitFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.material.SetFloat("_FlashAmount", 1);
            yield return new WaitForSeconds(hitFlashInterval);
            spriteRenderer.material.SetFloat("_FlashAmount", 0);
        }
    }
}
