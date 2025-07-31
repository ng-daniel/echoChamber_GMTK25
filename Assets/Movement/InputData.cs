using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputData
{

    Vector2 moveDirection;
    Vector2 aimDirection;
    bool dashPress;
    bool mouseHold;
    bool mouseClick; // active on frame of initial mouse click

    public InputData(Vector2 moveDirection, Vector2 aimDirection, bool dashPress, bool mouseHold, bool mouseClick)
    {
        this.moveDirection = moveDirection;
        this.aimDirection = aimDirection;
        this.dashPress = dashPress;
        this.mouseHold = mouseHold;
        this.mouseClick = mouseClick;
    }

    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }
    public Vector2 GetAimDirection()
    {
        return aimDirection;
    }
    public bool IsDashPress()
    {
        return dashPress;
    }
    public bool IsMouseHold()
    {
        return mouseHold;
    }
    public bool IsMouseClick()
    {
        return mouseClick;
    }

    public override string ToString()
    {
        return (
            "Input Data [" +
            "moveDirection: (" + moveDirection.x + ", " + moveDirection.y + ")" +
            "]"
         );
    }

}
