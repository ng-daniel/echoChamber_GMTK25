using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InputDataSystem;

public class EnemySpawnManager : MonoBehaviour
{

    InputDataRepository idr;
    [SerializeField] GameObject enemyPrefab;
    List<EnemyController> enemyList = new List<EnemyController>();

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
    // [SerializeField] float limitCheckInterval;
    // float limitCheckTimer;


    [Header("Testing")]
    [SerializeField] bool runStatic;
    [SerializeField] bool runDynamic;


    void Awake()
    {
        idr = FindFirstObjectByType<InputDataRepository>();

        GlobalEventHolder.OnInitialServerClear += InitializeSpawning;
        GlobalEventHolder.OnDeath += CheckPlayerDeadOnDeath;
    }
    void OnDestroy()
    {
        GlobalEventHolder.OnInitialServerClear -= InitializeSpawning;
        GlobalEventHolder.OnDeath -= CheckPlayerDeadOnDeath;
    }

    void Update()
    {
        if (runStatic) StaticSpawnChain();
        else if (runDynamic) DynamicSpawnChain();

        if (active && !(enemyList.Count > maxEnemies))
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnInterval)
            {
                DynamicSpawnChain();
                if (useDynamic)
                {
                    DynamicSpawnChain();
                }
                else
                {
                    StaticSpawnChain();
                }
                useDynamic = !useDynamic;
                spawnTimer = 0;
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

        InputDataMetadata meta = idr.GetInputBySecondsBehind(staticStartSecondsBehind);
        if (meta == null) return false;

        StartCoroutine(StaticSpawnChainCoroutine(meta));
        return true;
    }
    IEnumerator StaticSpawnChainCoroutine(InputDataMetadata meta)
    {
        staticSpawnRunning = true;
        for (int i = 0; i < staticSpawnAmount; i++)
        {
            SpawnEnemyWithInputFrame(meta);
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

        InputDataMetadata meta = idr.GetInputBySecondsBehind(staticStartSecondsBehind);
        if (meta == null) return false;

        List<InputDataMetadata> dataSequence = new List<InputDataMetadata>();
        int start = meta.GetIndex();
        int interval = Mathf.FloorToInt(dynamicSpawnDistanceSec * InputDataRepository.DATA_PER_SECOND);
        int end = start + (interval * dynamicSpawnAmount) - 1;
        for (int i = start; i < end; i += interval)
        {
            InputDataMetadata d = idr.GetInput(i);
            if (d == null) return false;
            dataSequence.Add(d);
        }
        StartCoroutine(DynamicSpawnChainCoroutine(dataSequence));
        return true;
    }
    IEnumerator DynamicSpawnChainCoroutine(List<InputDataMetadata> dataSequence)
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

    void SpawnEnemyWithInputFrame(InputDataMetadata inputDataMetadata)
    {
        if (inputDataMetadata == null) print("ERROR: inputDataMetadata is null for trying to spawn an enemy.");

        GameObject newEnemy = Instantiate(enemyPrefab, inputDataMetadata.GetInputData().GetPosition(), Quaternion.identity);
        EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
        enemyController.Initialize(inputDataMetadata, enemyList.Remove);
        enemyList.Add(enemyController);
    }
    void CheckPlayerDeadOnDeath(GameObject victim)
    {
        if (victim.CompareTag("Player") || victim.CompareTag("Boss"))
        {
            StopAllCoroutines();
            active = false;
            // GibbAllEnemies();
        }
    }
    void GibbAllEnemies()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            EnemyController victim = enemyList[i];
            if (victim != null) victim.TriggerDeath();
        }
    }
    void LimitEnemiesFunction()
    {
        while (enemyList.Count > maxEnemies)
        {
            print("Limiting Enemies!");
            EnemyController victim = enemyList[0];
            if (victim != null) victim.TriggerDeath();
        }
    }
    // public void CheckAndRemoveEnemyOnDeath(GameObject victim)
    // {
    //     if (victim.GetComponent<EnemyController>() != null && enemyList.Contains(victim)) enemyList.Remove(victim);
    // }
}
