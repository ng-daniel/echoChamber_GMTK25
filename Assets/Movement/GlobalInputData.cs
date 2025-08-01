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

    public void RecordInput(InputData dataPoint)
    {
        inputData.Add(dataPoint);
    }

    public InputData GetInput(int index)
    {
        return inputData[index];
    }

    void FixedUpdate()
    {
        //print(inputData.Count);
    }

    public void VerifyLowestIndex(int accessedLow)
    {

    }
}
