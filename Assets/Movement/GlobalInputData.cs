using System.Collections;
using System.Collections.Generic;
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

    List<InputData> inputData = new List<InputData>();
    int lowestIndex;

    public void RecordInput(InputData dataPoint)
    {
        inputData.Add(dataPoint);
    }

    void FixedUpdate()
    {
        print(inputData.Count);
    }

    public void VerifyLowestIndex(int accessedLow)
    {

    }
}
