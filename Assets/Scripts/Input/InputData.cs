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
    int hotswapValue; // -1 if no swapping occured, else values 0,1,2,...,8,9 corresponding to keys 1,2,3,...,9,0

    public InputData(InputData data)
    {
        index = data.index;
        moveDirection = data.moveDirection;
        aimDirection = data.aimDirection;
        position = data.position;
        dashPress = data.dashPress;
        mouseHold = data.mouseHold;
        mouseClick = data.mouseClick;
        hotswapValue = data.hotswapValue;
    }
    public InputData(Vector2 moveDirection, Vector2 aimDirection, Vector3 position, bool dashPress, bool mouseHold, bool mouseClick, int hotswapValue)
    {
        this.moveDirection = moveDirection;
        this.aimDirection = aimDirection;
        this.position = position;
        this.dashPress = dashPress;
        this.mouseHold = mouseHold;
        this.mouseClick = mouseClick;
        this.hotswapValue = hotswapValue;

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
    public int GetHotSwapIdx()
    {
        return hotswapValue;
    }

    public override string ToString()
    {
        return "Input_" + index + " [(" + moveDirection.x + ", " + moveDirection.y + "), <" + aimDirection.x + ", " + aimDirection.y + ">]";
    }

}
