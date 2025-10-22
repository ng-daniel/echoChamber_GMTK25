using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    CharacterStateManager characterStateManager;

    [Header("Read Input Parameters")]
    [SerializeField] int index;
    bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        characterStateManager = GetComponent<CharacterStateManager>();
    }

    public void Initialize(InputData firstMove)
    {
        transform.position = firstMove.GetPosition();
        this.index = firstMove.GetIndex();
    }
    void FixedUpdate()
    {
        if (isActive)
        {
            InputData nextMove = GlobalInputData.GetInstance().GetInput(index);
            characterStateManager.HandleInputs(nextMove);
            index++;
        }
    }
    public void ActivateEnemy()
    {
        isActive = true;
        characterStateManager.ActivateCharacter();
    }
}
