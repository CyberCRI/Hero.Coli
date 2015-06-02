using UnityEngine;
using System.Collections;

public class LeftClickToMoveMainMenuItem : ControlMainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will left click to move...");
        controlsArray.switchControlTypeToLeftClickToMove();
    }
}
