using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{

    GameObject player;
    PlayerInput input;
    InputReciever reciever;
    InputAction moveAction;
    InputAction dashAction;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        input = GetComponent<PlayerInput>();
        reciever = player.GetComponent<InputReciever>();
        moveAction = input.actions.FindAction("Move");
        dashAction = input.actions.FindAction("Dash");
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) SendPlayerCharacterInputs();
    }

    void SendPlayerCharacterInputs()
    {
        Vector2 mouseWorldDirection = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).normalized;
        Vector2 moveDirection = moveAction.ReadValue<Vector2>();
        bool dashPress = dashAction.ReadValue<float>() != 0;
        bool mouseHold = Mouse.current.leftButton.isPressed;
        bool mouseClick = Mouse.current.leftButton.wasPressedThisFrame;

        InputData data = new(
            moveDirection,
            mouseWorldDirection,
            dashPress,
            mouseHold,
            mouseClick
        );
        reciever.HandleInputs(data);
    }
}
