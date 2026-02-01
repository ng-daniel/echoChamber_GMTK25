using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace InputDataSystem
{
    public class InputDataRepository : MonoBehaviour
    {
        [SerializeField] Dictionary<int, List<InputDataMetadata>> inputData = new Dictionary<int, List<InputDataMetadata>>();
        public const float SEC_PER_BIN = 0.2f;
        int currentBinIdx = 0; // represents some number of of input data
        float timer = 0;
        public const int DATA_PER_SECOND = 50; // due to fixed update always running 50 times per second
        [SerializeField] int maxTimeSeconds; // maximum history in seconds to store
        [SerializeField] float cleanseInterval;
        float cleanseTimer;
        [SerializeField] int safeDataAccessBufferTimeSeconds;

        public void RecordInput(InputData dataPoint)
        {
            List<InputDataMetadata> binList;
            if (!inputData.ContainsKey(currentBinIdx))
            {
                inputData[currentBinIdx] = new List<InputDataMetadata>();
            }
            binList = inputData[currentBinIdx];

            InputDataMetadata newMeta = new(currentBinIdx, dataPoint);
            binList.Last()?.SetNext(newMeta);
            binList.Add(newMeta);
        }
        public InputDataMetadata GetInput(int binIndex)
        {
            if (inputData.TryGetValue(binIndex, out List<InputDataMetadata> data))
            {
                return data[0];
            }
            return null;
        }
        public int GetCurrentBinIndex()
        {
            return currentBinIdx;
        }
        public InputDataMetadata GetNextInput(InputDataMetadata meta)
        {
            InputDataMetadata nextMeta = meta.GetNext();
            if (nextMeta != null)
            {
                return nextMeta;
            }
            else
            {
                int nextBin = nextMeta.GetIndex();
                return inputData[nextBin][0];
            }
        }
        public InputDataMetadata GetInputBySecondsBehind(int secondsBehind)
        {
            int targetBin = currentBinIdx - (int)Mathf.Floor(secondsBehind / SEC_PER_BIN);
            if (inputData.TryGetValue(targetBin, out List<InputDataMetadata> data))
            {
                return data[0];
            }
            return null;
        }
        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= SEC_PER_BIN)
            {
                currentBinIdx++;
                timer = 0;
            }
        }
        void FixedUpdate()
        {
            // print("DATA: " + inputData.Keys.Min() + " | " + GetEarliestSafeIndex() + " | " + inputData.Keys.Max() + " | " + (GetEarliestSafeInput() == null));
            // cleanseTimer += Time.deltaTime;
            // if (cleanseTimer > cleanseInterval)
            // {
            //     CleanseDataList();
            //     cleanseTimer = 0;
            // }
        }
        // void CleanseDataList()
        // {
        //     // everytime cleanse data is called, remove data over the limit

        //     int extraData = GetExtraDataCount();
        //     if (extraData > 0)
        //     {
        //         int min = inputData.Keys.Min();
        //         for (int i = min; i < (min + extraData); i++)
        //         {
        //             inputData.Remove(i);
        //         }
        //     }
        // }
        // int GetExtraDataCount()
        // {
        //     int extraData = inputData.Count - (maxTimeSeconds * DATA_PER_SECOND);
        //     return extraData > 0 ? extraData : 0;
        // }
    }
}