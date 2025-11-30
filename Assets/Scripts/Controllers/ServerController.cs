using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController : MonoBehaviour
{

    Animator anim;
    Health health;
    bool isOperating;

    void Start()
    {
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
        health.SetDeathEvent(StopOperatingServer);
    }

    public bool IsOperating()
    {
        return isOperating;
    }
    public void ResetServer()
    {
        anim.ResetTrigger("start");
        anim.ResetTrigger("stop");
        isOperating = false;
    }

    public void StartOperatingServer()
    {
        isOperating = true;
        anim.SetTrigger("start");
        health.SetHealth(health.GetMaxHealth());
        health.ClearFilterList();
    }
    void StopOperatingServer()
    {
        isOperating = false;
        anim.SetTrigger("stop");
    }
}
