using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class GlobalInputData : MonoBehaviour
{

    private static GlobalInputData instance;
    private GlobalInputData() { }
    public static GlobalInputData GetInstance()
    {
        return instance;
    }
    void Start()
    {
        instance = this;
    }

    [SerializeField] Dictionary<int, InputData> inputData = new Dictionary<int, InputData>();
    public const int DATA_PER_SECOND = 50; // due to fixed update always running 50 times per second
    [SerializeField] int maxTimeSeconds; // maximum history in seconds to store
    [SerializeField] float cleanseInterval;
    float cleanseTimer;
    [SerializeField] int safeDataAccessBufferTimeSeconds;

    public void RecordInput(InputData dataPoint)
    {
        inputData.Add(dataPoint.GetIndex(), dataPoint);
    }

    public InputData GetInput(int index)
    {
        InputData data;
        if (inputData.TryGetValue(index, out data))
        {
            return new(data);
        }
        return null;
    }
    public InputData GetEarliestInput()
    {
        return GetInput(inputData.Keys.Min());
    }
    public InputData GetEarliestSafeInput()
    {
        return GetInput(GetEarliestSafeIndex());
    }
    public int GetEarliestSafeIndex()
    {
        return inputData.Keys.Min() + GetExtraDataCount() + (safeDataAccessBufferTimeSeconds * DATA_PER_SECOND);
    }
    public InputData GetInputBySecondsBehind(int seconds)
    {
        return GetInput(inputData.Keys.Max() - (seconds * DATA_PER_SECOND));
    }

    void FixedUpdate()
    {
        // print("DATA: " + inputData.Keys.Min() + " | " + GetEarliestSafeIndex() + " | " + inputData.Keys.Max() + " | " + (GetEarliestSafeInput() == null));
        cleanseTimer += Time.deltaTime;
        if (cleanseTimer > cleanseInterval)
        {
            CleanseDataList();
            cleanseTimer = 0;
        }
    }

    void CleanseDataList()
    {
        // everytime cleanse data is called, remove data over the limit

        int extraData = GetExtraDataCount();
        if (extraData > 0)
        {
            int min = inputData.Keys.Min();
            for (int i = min; i < (min + extraData); i++)
            {
                inputData.Remove(i);
            }
        }
    }
    int GetExtraDataCount()
    {
        int extraData = inputData.Count - (maxTimeSeconds * DATA_PER_SECOND);
        return extraData > 0 ? extraData : 0;
    }
}
