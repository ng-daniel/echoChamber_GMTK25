using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputDataSystem
{
    public class PlayerInputHandler : MonoBehaviour
    {

        GameObject player;
        PlayerInput input;
        InputDataRepository idr;
        PlayerController playerController;
        InputAction moveAction;
        InputAction dashAction;
        InputAction hotswapAction;
        InputData currentInput;
        Vector2 mouseWorldLocation;

        // buffer vars
        int mostRecentHotswap = 0;
        bool recentMouseClick = false;


        void Awake()
        {
            idr = FindFirstObjectByType<InputDataRepository>();

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
            // buffer mouse clicks
            Tuple<bool, bool> mouseInputs = ReadMouseInputs();
            bool mouseHold = mouseInputs.Item1;
            bool mouseClick = mouseInputs.Item2;

            // buffer hotswap inputs
            int hotswapIdx = ReadHotswapInput();
            if (hotswapIdx != -1 && hotswapIdx != mostRecentHotswap)
            {
                mostRecentHotswap = hotswapIdx;
            }

            if (mouseClick && !recentMouseClick)
            {
                recentMouseClick = true;
            }
        }
        void FixedUpdate()
        {
            if (player != null)
            {
                currentInput = GenerateCurrentPlayerInput();
                currentInput.SetMouseClick(recentMouseClick);
                recentMouseClick = false;
                TrySendPlayerCharacterInputs(currentInput);
            }
        }

        Tuple<bool, bool> ReadMouseInputs()
        {
            return new Tuple<bool, bool>(
                Mouse.current.leftButton.isPressed,
                Mouse.current.leftButton.wasPressedThisFrame
            );
        }
        int ReadHotswapInput()
        {
            int hotswapIdx = -1;
            if (hotswapAction.triggered)
            {
                float rawValue = hotswapAction.ReadValue<float>();
                hotswapIdx = (int)rawValue - 1; // since raw input is read as 1-10, adjust for indexing by subtracting 1 to make it 0-9 (not pressing anything gives -1)
                print("TRIGGERED: " + hotswapIdx);
            }
            return hotswapIdx;
        }

        InputData GenerateCurrentPlayerInput()
        {
            // read mouse data
            Vector2 mouseWorldDirection = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - player.transform.position;
            SetMousePosition((Vector3)mouseWorldDirection + player.transform.position);

            Tuple<bool, bool> mouseInputs = ReadMouseInputs();
            bool mouseHold = mouseInputs.Item1;
            bool mouseClick = mouseInputs.Item2;

            // read movement data
            Vector2 moveDirection = moveAction.ReadValue<Vector2>();
            bool dashPress = dashAction.ReadValue<float>() != 0;

            // read swap data
            int hotswapIdx = ReadHotswapInput();
            if (hotswapIdx != -1 && hotswapIdx != mostRecentHotswap)
            {
                mostRecentHotswap = hotswapIdx;
            }

            InputData data = new(
                moveDirection,
                mouseWorldDirection.normalized,
                player.transform.position,
                dashPress,
                mouseHold,
                mouseClick,
                mostRecentHotswap
            );
            return data;
        }
        void TrySendPlayerCharacterInputs(InputData data)
        {
            bool result = playerController.HandleInputs(data);
            if (result == true)
            {
                idr.RecordInput(data);
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
}