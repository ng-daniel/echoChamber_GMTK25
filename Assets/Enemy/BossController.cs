using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{

    Health health;
    [SerializeField] GameObject laser;
    [SerializeField] Transform laserOriginPoint;
    [SerializeField] LaserParameters rotateLaserParam;

    [Header("Attack Params")]
    bool attackEnabled;
    [SerializeField] float attackIntervalSeconds;
    [SerializeField] float attackTimer;
    [SerializeField] float laserStartAngleOffset;

    [Header("Shield Params")]
    [SerializeField] bool shieldEnabled;

    [Header("Server Params")]
    [SerializeField] GameObject serverPrefab;
    [SerializeField] List<Transform> serverPositions = new List<Transform>();
    List<GameObject> serverList = new List<GameObject>();
    [SerializeField] float serverDownTimeSec;
    [SerializeField] int minServerOperate;
    [SerializeField] int maxServerOperate;
    [SerializeField] float serverDownTimer;
    bool initialOperate;

    void Start()
    {
        health = GetComponent<Health>();
        InitializeServers();
        //SetAttackEnabled(true);
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
        Vector2 initialDirection = Vector2.down;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            initialDirection = (player.transform.position - transform.position).normalized;
        }
        initialDirection = Quaternion.AngleAxis(laserStartAngleOffset, Vector3.forward) * initialDirection;

        GameObject newLaser = Instantiate(laser, laserOriginPoint.position, Quaternion.identity);
        LaserScript newLaserScript = newLaser.GetComponent<LaserScript>();
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
    }

    public void DeathEvent()
    {
        GlobalEventHolder.OnDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }
    public Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }
}
