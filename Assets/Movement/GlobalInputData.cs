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

    Dictionary<int, InputData> inputData = new Dictionary<int, InputData>();
    const int DATA_PER_SECOND = 50; // due to fixed update always running 50 times per second
    [SerializeField] int maxTimeSeconds; // maximum history in seconds to store
    [SerializeField] float cleanseInterval;
    float cleanseTimer;

    public void RecordInput(InputData dataPoint)
    {
        inputData.Add(dataPoint.GetIndex(), dataPoint);
    }

    public InputData GetInput(int index)
    {
        if (index < inputData.Keys.Min() || index > inputData.Keys.Max()) return null;
        return inputData[index];
    }
    public InputData GetEarliestInput()
    {
        return GetInput(inputData.Keys.Min());
    }
    public InputData GetInputBySecondsBehind(int seconds)
    {
        int index = inputData.Keys.Max() - seconds * DATA_PER_SECOND;
        return GetInput(index);
    }

    void FixedUpdate()
    {
        //print(inputData.Count);
    }

    void CleanseDataList()
    {
        // everytime cleanse data is called, remove some of the data over the limit

        float proportion = 0.9f;
        int extraData = inputData.Count - (maxTimeSeconds * DATA_PER_SECOND);
        if (extraData > 0)
        {
            int min = inputData.Keys.Min();
            for (int i = min; i < (min + Mathf.RoundToInt(extraData * proportion)); i++)
            {
                inputData.Remove(i);
            }
        }
    }
}
