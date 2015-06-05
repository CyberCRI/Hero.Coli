using UnityEngine;
using System.Collections;

public class LeftClickToMoveMainMenuItem : MouseControlMainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will left click to move...");
        controlsArray.switchControlTypeToLeftClickToMove();
    }
}
