using System;
using UnityEngine;

namespace InputDataSystem
{
    public class InputDataMetadata
    {
        int binIndex;
        InputDataMetadata next = null;
        InputData inputData;
        public InputDataMetadata(int binIndex, InputData inputData)
        {
            this.binIndex = binIndex;
            this.inputData = inputData;
        }
        public int GetIndex()
        {
            return binIndex;
        }
        public InputData GetInputData()
        {
            return inputData;
        }
        public InputDataMetadata GetNext()
        {
            return next;
        }
        public void SetNext(InputDataMetadata next)
        {
            this.next = next;
        }
    }
}