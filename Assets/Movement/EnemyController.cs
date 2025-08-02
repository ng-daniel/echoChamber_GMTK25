using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    GlobalInputData globalInput;
    CharacterStateManager characterStateManager;

    [Header("Read Input Parameters")]
    [SerializeField] int index;
    bool startLoop = false;

    // Start is called before the first frame update
    void Start()
    {
        globalInput = GlobalInputData.GetInstance();
        characterStateManager = GetComponent<CharacterStateManager>();
    }

    void FixedUpdate()
    {
        InputData nextMove = globalInput.GetInput(index);
        characterStateManager.HandleInputs(nextMove);
        index++;
    }
    public void Initialize()
    {
        InputData nextMove = globalInput.GetEarliestInput();
        transform.position = nextMove.GetPosition();
        this.index = nextMove.GetIndex();
    }

    void StartLoop()
    {
        startLoop = true;
    }
}
