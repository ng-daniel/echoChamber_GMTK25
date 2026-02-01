using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    InputDataRepository idm;
    EchoStateManager esm;
    Health health;
    SpriteRenderer sprite;
    Animator anim;

    [Header("Read Input Parameters")]
    int index;
    bool isActive;
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

    public void Initialize(InputData firstMove, Func<EnemyController, bool> removeFromRecord)
    {
        transform.position = firstMove.GetPosition();
        this.index = firstMove.GetIndex();
        this.removeFromRecord = removeFromRecord;
    }
    void FixedUpdate()
    {
        if (isActive)
        {
            InputData nextMove = idm.GetInput(index);
            esm.HandleInputs(nextMove);
            index++;
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
