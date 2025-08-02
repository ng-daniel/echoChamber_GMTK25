using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{

    [SerializeField] GameObject enemyPrefab;
    static List<GameObject> enemyList = new List<GameObject>();

    [Header("Static Spawn Chain Params")]
    bool staticSpawnRunning = false;
    [SerializeField] int startSecondsBehind;
    [SerializeField] float staticSpawnIntervalSec;
    [SerializeField] int staticSpawnAmount;


    void Update()
    {
        StaticSpawnChain();
    }

    public void StaticSpawnChain()
    {
        if (staticSpawnRunning) return;
        StartCoroutine(StaticSpawnChainCoroutine());
    }
    IEnumerator StaticSpawnChainCoroutine()
    {
        staticSpawnRunning = true;
        InputData data = GlobalInputData.GetInstance().GetInputBySecondsBehind(startSecondsBehind);
        if (data != null)
        {
            for (int i = 0; i < staticSpawnAmount; i++)
            {
                print("DATA: " + data.GetIndex());
                SpawnEnemyWithInputFrame(data);
                yield return new WaitForSeconds(staticSpawnIntervalSec);
            }
        }
        staticSpawnRunning = false;
    }

    void SpawnEnemyWithInputFrame(InputData inputData)
    {
        if (inputData == null) print("ERROR: inputData is null for trying to spawn an enemy.");

        GameObject newEnemy = Instantiate(enemyPrefab, inputData.GetPosition(), Quaternion.identity);
        EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
        enemyController.Initialize();
        enemyList.Add(newEnemy);
    }

    public static void OnEnemyDeath(GameObject deadEnemy)
    {
        // actions
        enemyList.Remove(deadEnemy);
    }
}
