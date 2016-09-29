using UnityEngine;
using System.Collections;

public class LeftClickToMoveMainMenuItem : MouseControlMainMenuItem {
    public override void click() {
        // Debug.Log(this.GetType());
        controlsArray.switchControlTypeToLeftClickToMove();
    }
}
