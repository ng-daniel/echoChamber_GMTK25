using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
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

    void FixedUpdate()
    {
        InputData nextMove = globalInput.GetInput(currentStep);
        nextMove.SetAimDirection(nextMove.GetAimDirection() * -1);
        characterStateManager.HandleInputs(nextMove);
        currentStep++;
    }

    void StartLoop()
    {
        startLoop = true;
    }

    void SetAnimator(Animator animator)
    {

    }

}
