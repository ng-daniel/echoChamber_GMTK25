using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{

    Health health;
    Animator anim;
    [SerializeField] GameObject laser;
    [SerializeField] Transform laserOriginPoint;
    [SerializeField] LaserParameters rotateLaserParam;

    [Header("Attack Params")]
    bool attackEnabled;
    [SerializeField] float attackIntervalSeconds;
    [SerializeField] float attackTimer;
    [SerializeField] float laserStartAngleOffset;
    GameObject laserInstance;

    [Header("Shield Params")]
    [SerializeField] public bool shieldEnabled;
    [SerializeField] GameObject shieldVisual;
    [SerializeField] GameObject deathVisual;

    [Header("Server Params")]
    [SerializeField] GameObject serverPrefab;
    [SerializeField] List<Transform> serverPositions = new List<Transform>();
    List<GameObject> serverList = new List<GameObject>();
    [SerializeField] float serverDownTimeSec;
    [SerializeField] int minServerOperate;
    [SerializeField] int maxServerOperate;
    [SerializeField] float serverDownTimer;
    bool initialOperate;
    bool initialServerClear;

    void Start()
    {
        health = GetComponent<Health>();
        health.SetDeathEvent(DeathEvent);
        anim = GetComponent<Animator>();
        InitializeServers();
    }

    void Update()
    {
        if (!initialOperate)
        {
            initialOperate = true;
            OperateServers(serverList.Count);
        }

        if (attackEnabled)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > attackIntervalSeconds)
            {
                LaserSpin();
                attackTimer = 0;
            }
        }

        bool serversOperational = CheckServersOperational();
        if (shieldEnabled != serversOperational)
        {
            print("adjusting invulnerability");
            SetShieldEnabled(serversOperational);
            AdjustInvulnerability(shieldEnabled);
        }

        if (!shieldEnabled)
        {
            if (!initialServerClear)
            {
                GlobalEventHolder.OnInitialServerClear?.Invoke();
                SetAttackEnabled(true);
                initialServerClear = true;
            }

            serverDownTimer += Time.deltaTime;
            if (serverDownTimer > serverDownTimeSec)
            {
                int num = Random.Range(minServerOperate, maxServerOperate + 1);
                OperateServers(num);
                serverDownTimer = 0;
            }
        }
    }

    void LaserSpin()
    {
        print("FIRING LASER");
        float randomAngle = Random.Range(0, 360);
        Vector2 initialDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized;
        print(initialDirection);
        laserInstance = Instantiate(laser, laserOriginPoint.position, Quaternion.identity);
        LaserScript newLaserScript = laserInstance.GetComponent<LaserScript>();
        newLaserScript.Initialize(this.gameObject, rotateLaserParam);
        newLaserScript.RotateAttackChain(initialDirection);
    }

    void InitializeServers()
    {
        for (int i = 0; i < serverPositions.Count; i++)
        {
            serverList.Add(Instantiate(serverPrefab, serverPositions[i].position, Quaternion.identity));
        }
    }
    void OperateServers(int numServers)
    {
        print("OPERATING SERVERS");
        List<GameObject> tempServers = new List<GameObject>(serverList);
        for (int i = 0; i < numServers; i++)
        {
            if (tempServers.Count == 0)
            {
                print("BossControllerOperateServers ERROR: Not enough servers to satisfy numSevers input!");
                break;
            }
            int index = Random.Range(0, tempServers.Count);
            ServerController server = tempServers[index].GetComponent<ServerController>();
            server.ResetServer();
            server.StartOperatingServer();
            tempServers.RemoveAt(index);
        }
    }
    bool CheckServersOperational()
    {
        foreach (GameObject server in serverList)
        {
            if (server.GetComponent<ServerController>().IsOperating()) return true;
        }
        return false;
    }
    public int GetOperatingServersCount()
    {
        int count = 0;
        foreach (GameObject server in serverList)
        {
            if (server.GetComponent<ServerController>().IsOperating()) count++;
        }
        return count;
    }
    void AdjustInvulnerability(bool val)
    {
        if (val) health.damageFilterList.Add(BossInvunlerabilityDamageFilter);
        else health.damageFilterList.Remove(BossInvunlerabilityDamageFilter);
    }
    DamageData BossInvunlerabilityDamageFilter(DamageData data)
    {
        print("BOSS INVINCIBILITY");
        data.SetDamage(0);
        return data;
    }

    public void SetAttackEnabled(bool val)
    {
        attackEnabled = val;
    }
    public void SetShieldEnabled(bool val)
    {
        shieldEnabled = val;
        shieldVisual.SetActive(val);
        anim.SetBool("shieldEnabled", val);
    }

    public void DeathEvent()
    {
        GlobalEventHolder.OnDeath?.Invoke(gameObject);
        if (laserInstance != null) Destroy(laserInstance);
        Instantiate(deathVisual, transform.position, Quaternion.identity);
        //SceneManager.SetActiveScene()
        Destroy(gameObject);
    }
    public Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }
}
