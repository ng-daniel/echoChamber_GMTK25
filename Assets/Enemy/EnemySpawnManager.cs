using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{

    [SerializeField] GameObject enemyPrefab;
    static List<GameObject> enemyList = new List<GameObject>();

    [Header("Static Spawn Chain Params")]
    bool staticSpawnRunning = false;
    [SerializeField] int staticStartSecondsBehind;
    [SerializeField] float staticSpawnIntervalSec;
    [SerializeField] int staticSpawnAmount;

    [Header("Dynamic Spawn Chain Params")]
    bool dynamicSpawnRunning = false;
    [SerializeField] int dynamicStartSecondsBehind;
    [SerializeField] float dynamicSpawnIntervalSec;
    [SerializeField] float dynamicSpawnDistanceSec;
    [SerializeField] int dynamicSpawnAmount;

    [Header("Testing")]
    [SerializeField] bool runStatic;
    [SerializeField] bool runDynamic;


    void Update()
    {
        if (runStatic) StaticSpawnChain();
        else if (runDynamic) DynamicSpawnChain();
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
        //staticSpawnRunning = false;
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

    public static void OnEnemyDeath(GameObject deadEnemy)
    {
        // actions
        enemyList.Remove(deadEnemy);
    }
}
