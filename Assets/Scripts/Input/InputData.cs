using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputData
{
    static int nextIndex = 0;
    int index;
    Vector2 moveDirection;
    Vector2 aimDirection;
    Vector3 position;
    bool dashPress;
    bool mouseHold;
    bool mouseClick; // active on frame of initial mouse click

    public InputData(InputData data)
    {
        index = data.index;
        moveDirection = data.moveDirection;
        aimDirection = data.aimDirection;
        position = data.position;
        dashPress = data.dashPress;
        mouseHold = data.mouseHold;
        mouseClick = data.mouseClick;
    }
    public InputData(Vector2 moveDirection, Vector2 aimDirection, Vector3 position, bool dashPress, bool mouseHold, bool mouseClick)
    {
        this.moveDirection = moveDirection;
        this.aimDirection = aimDirection;
        this.position = position;
        this.dashPress = dashPress;
        this.mouseHold = mouseHold;
        this.mouseClick = mouseClick;

        index = nextIndex;
        nextIndex++;
    }

    public int GetIndex()
    {
        return index;
    }
    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }
    public Vector2 GetAimDirection()
    {
        return aimDirection;
    }
    public void SetAimDirection(Vector2 aimDirection)
    {
        this.aimDirection = aimDirection;
    }
    public Vector2 GetPosition()
    {
        return position;
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
        return "Input_" + index + " [(" + moveDirection.x + ", " + moveDirection.y + "), <" + aimDirection.x + ", " + aimDirection.y + ">]";
    }

}
