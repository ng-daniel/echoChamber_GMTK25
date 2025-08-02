using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    CharacterStateManager characterStateManager;

    [Header("Read Input Parameters")]
    [SerializeField] int index;
    bool startLoop = false;

    // Start is called before the first frame update
    void Start()
    {
        characterStateManager = GetComponent<CharacterStateManager>();
    }

    void FixedUpdate()
    {
        InputData nextMove = GlobalInputData.GetInstance().GetInput(index);
        characterStateManager.HandleInputs(nextMove);
        index++;
    }
    public void Initialize()
    {
        InputData nextMove = GlobalInputData.GetInstance().GetEarliestSafeInput();
        transform.position = nextMove.GetPosition();
        this.index = nextMove.GetIndex();
    }

    void StartLoop()
    {
        startLoop = true;
    }
}
