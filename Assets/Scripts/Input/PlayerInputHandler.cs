using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{

    GameObject player;
    PlayerInput input;
    PlayerController playerController;
    InputAction moveAction;
    InputAction dashAction;
    InputAction hotswapAction;
    InputData currentInput;
    Vector2 mouseWorldLocation;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        input = GetComponent<PlayerInput>();
        playerController = player.GetComponent<PlayerController>();
        moveAction = input.actions.FindAction("Move");
        dashAction = input.actions.FindAction("Dash");
        hotswapAction = input.actions.FindAction("HotSwap");
    }
    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            currentInput = GenerateCurrentPlayerInput();
            TrySendPlayerCharacterInputs(currentInput);
        }
    }

    InputData GenerateCurrentPlayerInput()
    {
        // read mouse data
        Vector2 mouseWorldDirection = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - player.transform.position;
        SetMousePosition((Vector3)mouseWorldDirection + player.transform.position);
        bool mouseHold = Mouse.current.leftButton.isPressed;
        bool mouseClick = Mouse.current.leftButton.wasPressedThisFrame;

        // read movement data
        Vector2 moveDirection = moveAction.ReadValue<Vector2>();
        bool dashPress = dashAction.ReadValue<float>() != 0;

        // read swap data
        int hotswapIdx = -1;
        if (hotswapAction.triggered)
        {
            float rawValue = hotswapAction.ReadValue<float>();
            hotswapIdx = (int)rawValue - 1; // since raw input is read as 1-9, adjust for indexing by subtracting 1
            print("TRIGGERED: " + hotswapIdx);
        }

        InputData data = new(
            moveDirection,
            mouseWorldDirection.normalized,
            player.transform.position,
            dashPress,
            mouseHold,
            mouseClick,
            hotswapIdx
        );
        return data;
    }
    void TrySendPlayerCharacterInputs(InputData data)
    {
        bool result = playerController.HandleInputs(data);
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
