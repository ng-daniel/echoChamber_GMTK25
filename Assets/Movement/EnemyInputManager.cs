using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInputManager : MonoBehaviour
{

    GlobalInputData globalInput;
    CharacterStateManager characterStateManager;
    int currentStep = 0;
    bool startLoop = false;

    // Start is called before the first frame update
    void Start()
    {
        globalInput = GlobalInputData.GetInstance();
        characterStateManager = GetComponent<CharacterStateManager>();
    }

    void Update()
    {
        // InputData nextMove = globalInput.GetInput(currentStep);
        // characterStateManager.HandleInputs(nextMove);
        // currentStep++;
    }

    void StartLoop()
    {
        startLoop = true;
    }

    void SetAnimator(Animator animator)
    {

    }

}
