using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{

    GameObject player;
    PlayerInput input;
    CharacterStateManager playerStateManager;
    InputAction moveAction;
    InputAction dashAction;
    InputData currentInput;
    Vector2 mouseWorldLocation;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        input = GetComponent<PlayerInput>();
        playerStateManager = player.GetComponent<CharacterStateManager>();
        moveAction = input.actions.FindAction("Move");
        dashAction = input.actions.FindAction("Dash");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player != null)
        {
            currentInput = GenerateCurrentPlayerInput();
            TrySendPlayerCharacterInputs(currentInput);
        }
    }

    InputData GenerateCurrentPlayerInput()
    {
        Vector2 mouseWorldDirection = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - player.transform.position;
        SetMousePosition((Vector3)mouseWorldDirection + player.transform.position);
        bool mouseHold = Mouse.current.leftButton.isPressed;
        bool mouseClick = Mouse.current.leftButton.wasPressedThisFrame;

        Vector2 moveDirection = moveAction.ReadValue<Vector2>();
        bool dashPress = dashAction.ReadValue<float>() != 0;

        InputData data = new(
            moveDirection,
            mouseWorldDirection.normalized,
            player.transform.position,
            dashPress,
            mouseHold,
            mouseClick
        );
        return data;
    }
    void TrySendPlayerCharacterInputs(InputData data)
    {
        bool result = playerStateManager.HandleInputs(data);
        if (result == true)
        {
            GlobalInputData.GetInstance().RecordInput(data);
        }
    }
    public InputData GetCurrentInputData()
    {
        return currentInput;
    }
    public Vector2 GetCurrentMouseWorldPosition()
    {
        return mouseWorldLocation;
    }
    public void SetMousePosition(Vector2 val)
    {
        mouseWorldLocation = val;
    }
}
