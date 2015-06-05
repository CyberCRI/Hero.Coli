using UnityEngine;
using System.Collections;

public class RightClickToMoveMainMenuItem : MouseControlMainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will right click to move...");
        controlsArray.switchControlTypeToRightClickToMove();
    }
}
