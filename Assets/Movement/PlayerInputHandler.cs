using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{

    PlayerInput input;
    InputReciever reciever;
    InputAction moveAction;
    InputAction dashAction;


    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        reciever = GameObject.FindGameObjectWithTag("Player").GetComponent<InputReciever>();
        moveAction = input.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        InputData data = new InputData(moveAction.ReadValue<Vector2>(), Vector2.zero, false, false, false);
        print(data);
        reciever.HandleInputs(data);
    }
}
