using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{

    [SerializeField] GameObject enemyPrefab;
    List<GameObject> enemyList = new List<GameObject>();

    [Header("Static Spawn Chain Params")]
    [SerializeField] bool staticSpawnRunning = false;
    [SerializeField] int staticStartSecondsBehind;
    [SerializeField] float staticSpawnIntervalSec;
    [SerializeField] int staticSpawnAmount;

    [Header("Dynamic Spawn Chain Params")]
    [SerializeField] bool dynamicSpawnRunning = false;
    [SerializeField] int dynamicStartSecondsBehind;
    [SerializeField] float dynamicSpawnIntervalSec;
    [SerializeField] float dynamicSpawnDistanceSec;
    [SerializeField] int dynamicSpawnAmount;

    [Header("Spawning Control Params")]
    bool active;
    bool useDynamic;
    [SerializeField] float spawnInterval;
    float spawnTimer;

    [Header("Enemy Limit Params")]
    [SerializeField] int maxEnemies;
    [SerializeField] float limitCheckInterval;
    float limitCheckTimer;


    [Header("Testing")]
    [SerializeField] bool runStatic;
    [SerializeField] bool runDynamic;


    void Awake()
    {
        GlobalEventHolder.OnInitialServerClear += InitializeSpawning;
        GlobalEventHolder.OnDeath += CheckPlayerDeadOnDeath;
        GlobalEventHolder.OnDeath += CheckAndRemoveEnemyOnDeath;

    }
    void OnDestroy()
    {
        GlobalEventHolder.OnInitialServerClear -= InitializeSpawning;
        GlobalEventHolder.OnDeath -= CheckPlayerDeadOnDeath;
        GlobalEventHolder.OnDeath -= CheckAndRemoveEnemyOnDeath;
    }

    void Update()
    {
        if (runStatic) StaticSpawnChain();
        else if (runDynamic) DynamicSpawnChain();

        if (active)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnInterval)
            {
                if (useDynamic)
                {
                    DynamicSpawnChain();
                }
                else
                {
                    StaticSpawnChain();
                }
                spawnTimer = 0;
                useDynamic = !useDynamic;
            }

            limitCheckTimer += Time.deltaTime;
            if (limitCheckTimer > limitCheckInterval)
            {
                LimitEnemiesFunction();
                limitCheckTimer = 0;
            }
        }
    }

    public void InitializeSpawning()
    {
        print("INITIALIZED ENEMYSPAWN");
        active = true;
        useDynamic = false;
        spawnTimer = 0;
        DynamicSpawnChain();
    }

    public bool StaticSpawnChain()
    {
        if (staticSpawnRunning) return false;

        InputData data = GlobalInputData.GetInstance().GetInputBySecondsBehind(staticStartSecondsBehind);
        if (data == null) return false;

        StartCoroutine(StaticSpawnChainCoroutine(data));
        return true;
    }
    IEnumerator StaticSpawnChainCoroutine(InputData data)
    {
        staticSpawnRunning = true;
        for (int i = 0; i < staticSpawnAmount; i++)
        {
            SpawnEnemyWithInputFrame(data);
            yield return new WaitForSeconds(staticSpawnIntervalSec);
        }
        staticSpawnRunning = false;
    }

    public bool DynamicSpawnChain()
    {
        if (dynamicSpawnRunning) return false;

        if (Mathf.Ceil(dynamicSpawnAmount * dynamicSpawnDistanceSec) >= dynamicStartSecondsBehind)
        {
            print("EnemySpawnDynamicSpawnChain ERROR: Your spawn amount math does NOT add up. Lower spawnAmount or spawnDistanceSec.");
            return false;
        }

        InputData data = GlobalInputData.GetInstance().GetInputBySecondsBehind(staticStartSecondsBehind);
        if (data == null) return false;

        List<InputData> dataSequence = new List<InputData>();
        int start = data.GetIndex();
        int interval = Mathf.FloorToInt(dynamicSpawnDistanceSec * GlobalInputData.DATA_PER_SECOND);
        int end = start + (interval * dynamicSpawnAmount) - 1;
        for (int i = start; i < end; i += interval)
        {
            InputData d = GlobalInputData.GetInstance().GetInput(i);
            if (d == null) return false;
            dataSequence.Add(d);
        }
        StartCoroutine(DynamicSpawnChainCoroutine(dataSequence));
        return true;
    }
    IEnumerator DynamicSpawnChainCoroutine(List<InputData> dataSequence)
    {
        dynamicSpawnRunning = true;
        for (int i = 0; i < dynamicSpawnAmount; i++)
        {
            print(dataSequence[i]);
            SpawnEnemyWithInputFrame(dataSequence[i]);
            yield return new WaitForSeconds(dynamicSpawnIntervalSec);
        }
        dynamicSpawnRunning = false;
    }

    void SpawnEnemyWithInputFrame(InputData inputData)
    {
        if (inputData == null) print("ERROR: inputData is null for trying to spawn an enemy.");

        GameObject newEnemy = Instantiate(enemyPrefab, inputData.GetPosition(), Quaternion.identity);
        EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
        enemyController.Initialize(inputData);
        enemyList.Add(newEnemy);
    }
    void CheckPlayerDeadOnDeath(GameObject victim)
    {
        if (victim.CompareTag("Player") || victim.CompareTag("Boss"))
        {
            StopAllCoroutines();
            active = false;
            GibbAllEnemies();
        }
    }
    void GibbAllEnemies()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            GameObject victim = enemyList[i];
            InstaGibb(victim);
        }
    }
    void LimitEnemiesFunction()
    {
        while (enemyList.Count > maxEnemies)
        {
            print("Limiting Enemies!");
            GameObject victim = enemyList[0];
            enemyList.Remove(victim);
            if (victim != null) InstaGibb(victim);
        }
    }
    void InstaGibb(GameObject victim)
    {
        if (victim == null) return;
        victim.GetComponent<Health>().Damage(new DamageData(this.gameObject, 99999999)); // mods, ban this guy
    }

    public void CheckAndRemoveEnemyOnDeath(GameObject victim)
    {
        if (victim.GetComponent<EnemyController>() != null && enemyList.Contains(victim)) enemyList.Remove(victim);
    }
}
