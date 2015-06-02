using UnityEngine;
using System.Collections;

public class RightClickToMoveMainMenuItem : ControlMainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will right click to move...");
        controlsArray.switchControlTypeToRightClickToMove();
    }
}
