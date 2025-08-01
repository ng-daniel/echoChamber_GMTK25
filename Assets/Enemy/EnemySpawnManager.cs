using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{

    GlobalInputData globalInputData;
    [SerializeField] GameObject enemyPrefab;
    static List<GameObject> enemyList = new List<GameObject>();

    [Header("Spawn Machine Parameters")]
    [SerializeField] bool loopingSpawnEnabled = false;

    [Header("Spawn Time Parameters")]
    [SerializeField] float firstSpawnTimeSeconds;
    [SerializeField] float spawnIntervalSeconds;
    [SerializeField] float spawnIntervalVarianceSeconds;
    float spawnTimer;
    float variance = 0;

    [Header("Spawn Frame Parameters")]
    [SerializeField] int firstSpawnFrame;
    [SerializeField] int spawnFrameBackLogRange; // how many frames are required in GlobalInputData before
    int currentFrame;

    void Start()
    {
        StartFirstSpawn();
    }

    void Update()
    {
        if (loopingSpawnEnabled)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnIntervalSeconds)
            {
                TrySpawnEnemyAtInputFrame(currentFrame);
                IncrementCurrentFrame();
                UpdateVariance();
                spawnTimer = 0;
            }
        }
    }

    bool TrySpawnEnemyAtInputFrame(int frame)
    {
        InputData inputData = GlobalInputData.GetInstance().GetInput(frame);
        if (inputData == null) return false;
        SpawnEnemyWithInputFrame(inputData);
        return true;
    }

    void SpawnEnemyWithInputFrame(InputData input)
    {
        GameObject newEnemy = Instantiate(enemyPrefab, input.GetPosition(), Quaternion.identity);
        enemyList.Add(newEnemy);
    }

    public static void OnEnemyDeath(GameObject deadEnemy)
    {
        // actions
        enemyList.Remove(deadEnemy);
    }

    void StartFirstSpawn()
    {
        StartCoroutine(FirstSpawnCoroutine(firstSpawnTimeSeconds));
    }
    IEnumerator FirstSpawnCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        TrySpawnEnemyAtInputFrame(firstSpawnFrame);
        ToggleLoopingSpawn(true);
    }
    void ToggleLoopingSpawn(bool val)
    {
        loopingSpawnEnabled = val;
    }
    void IncrementCurrentFrame()
    {
        currentFrame = 0;
    }
    void UpdateVariance()
    {
        variance = Random.Range(-spawnIntervalVarianceSeconds, spawnIntervalVarianceSeconds);
    }
}
