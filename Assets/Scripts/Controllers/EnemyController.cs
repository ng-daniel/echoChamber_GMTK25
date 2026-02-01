using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InputDataSystem;

public class EnemyController : MonoBehaviour
{
    InputDataRepository idm;
    EchoStateManager esm;
    Health health;
    SpriteRenderer sprite;
    Animator anim;

    [Header("Read Input Parameters")]
    bool isActive;
    InputDataMetadata currentInputMeta;
    [SerializeField] float deathTimer;
    Func<EnemyController, bool> removeFromRecord;

    void Awake()
    {
        esm = GetComponent<EchoStateManager>();

        health = GetComponent<Health>();
        health.SetHitEvent(HitEvent);
        health.SetDeathEvent(DeathEvent);

        sprite = GetComponent<SpriteRenderer>();

        anim = GetComponent<Animator>();

        idm = FindFirstObjectByType<InputDataRepository>();

        GlobalEventHolder.OnDeath += CheckPlayerDeath;
    }
    void OnDisable()
    {
        GlobalEventHolder.OnDeath -= CheckPlayerDeath;
    }

    public void Initialize(InputDataMetadata firstMove, Func<EnemyController, bool> removeFromRecord)
    {
        currentInputMeta = firstMove;
        transform.position = firstMove.GetInputData().GetPosition();
        this.removeFromRecord = removeFromRecord;
    }
    void FixedUpdate()
    {
        if (isActive)
        {
            currentInputMeta = idm.GetNextInput(currentInputMeta);
            esm.HandleInputs(currentInputMeta.GetInputData());
        }
    }
    public void ActivateEnemy()
    {
        isActive = true;
        esm.Activate();
    }
    void HitEvent()
    {
        print("yeowch!!!");
    }
    public void CheckPlayerDeath(GameObject obj)
    {
        if (obj.CompareTag("Player") || obj.CompareTag("Player"))
        {
            DeathEvent();
        }
    }
    public void TriggerDeath()
    {
        health.KillNoRegard();
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
        removeFromRecord(this);
        Destroy(gameObject);
    }
}
