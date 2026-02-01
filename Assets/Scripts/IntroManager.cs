using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InputDataSystem;

public class IntroManager : MonoBehaviour
{

    bool wPress;
    bool sPress;
    bool aPress;
    bool dPress;
    bool spacePress;
    bool lmbPress;

    bool pressesCompleted;
    bool introCompleted;

    PlayerInputHandler playerInputHandler;

    [SerializeField] List<string> theRundown = new List<string>();

    void Start()
    {
        playerInputHandler = GameObject.FindFirstObjectByType<PlayerInputHandler>();
    }

    void Update()
    {

        if (!AllPressesValidated()) ValidatePresses();

    }
    void ValidatePresses()
    {
        if (playerInputHandler == null)
        {
            print("ERROR: [no player input handler found]");
            return;
        }

        InputData currentInput = playerInputHandler.GetCurrentInputData();

        // movement checks
        Vector2 move = currentInput.GetMoveDirection();
        if (move.x > 0) dPress = true;
        if (move.x < 0) aPress = true;
        if (move.y > 0) wPress = true;
        if (move.y < 0) sPress = true;

        // fire check
        if (currentInput.IsMouseHold()) lmbPress = true;

        // dash check
        if (currentInput.IsDashPress()) spacePress = true;
    }
    bool AllPressesValidated()
    {
        return wPress && sPress && aPress && dPress && spacePress && lmbPress;
    }


}
