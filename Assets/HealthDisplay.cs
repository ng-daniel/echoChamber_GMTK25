using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{


    public GameObject controlsUI;
    public GameObject gameUI;
    GameObject boss;
    GameObject player;

    public TMP_Text bossHP;
    public TMP_Text playerHP;
    public TMP_Text shieldUp;
    public TMP_Text serversLeft;
    bool gaming;

    void Awake()
    {
        GlobalEventHolder.OnInitialServerClear += StartGameUI;
    }
    void OnDestroy()
    {
        GlobalEventHolder.OnInitialServerClear -= StartGameUI;
    }

    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        string bhp = boss ? boss.GetComponent<Health>().GetHealth() + "" : ":)";
        string php = player ? player.GetComponent<Health>().GetHealth() + "" : ":(";
        bossHP.text = "MACHINE_HP: " + bhp;
        playerHP.text = "SELF_HP: " + php;

        string sUp = boss ? boss.GetComponent<MachineBossController>().shieldEnabled + "" : ":)";
        string sLeft = boss ? boss.GetComponent<MachineBossController>().GetOperatingServersCount() + "" : ":)";
        shieldUp.text = "MACHINE_SHIELD: " + sUp;
        serversLeft.text = "N_SERVERS: " + sLeft;
    }
    void StartGameUI()
    {
        controlsUI.SetActive(false);
        gameUI.SetActive(true);
    }


}
