using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    EchoStateManager esm;
    Health health;
    SpriteRenderer sprite;
    Animator anim;
    bool invulnerable;
    [SerializeField] float invulnerableDuration;
    [SerializeField] float deathTimer;
    [SerializeField] float hitFlashInterval;

    // Start is called before the first frame update
    void Awake()
    {
        esm = GetComponent<EchoStateManager>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        health = GetComponent<Health>();
        health.SetHitEvent(HitEvent);
        health.SetDeathEvent(DeathEvent);
    }
    public void ActivatePlayer()
    {
        esm.Activate();
    }
    void HitEvent()
    {
        StartCoroutine(InvulnerableCoroutine());
    }
    public bool HandleInputs(InputData inputData)
    {
        return esm.HandleInputs(inputData);
    }
    IEnumerator InvulnerableCoroutine()
    {
        invulnerable = true;
        health.damageFilterList.Add(InvulnerableFrameDamageFilter);
        StartCoroutine(BlinkingFlash());

        yield return new WaitForSeconds(invulnerableDuration);

        health.damageFilterList.Remove(InvulnerableFrameDamageFilter);
        invulnerable = false;
    }
    IEnumerator BlinkingFlash()
    {
        while (invulnerable == true)
        {
            sprite.material.SetFloat("_FlashAmount", 1);
            yield return new WaitForSeconds(hitFlashInterval);
            sprite.material.SetFloat("_FlashAmount", 0);
            yield return new WaitForSeconds(hitFlashInterval);
            yield return null;
        }
    }
    DamageData InvulnerableFrameDamageFilter(DamageData data)
    {
        data.SetDamage(0);
        return data;
    }

    void DeathEvent()
    {
        anim.SetTrigger("death");
        esm.Deactivate();
        GlobalEventHolder.OnDeath?.Invoke(gameObject);
        StartCoroutine(DeathCoroutine());
    }
    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(deathTimer);
        SceneScript.GetInstance().SetSceneAfterTime("LoseScene", 1f);
        Destroy(gameObject);
    }
}
