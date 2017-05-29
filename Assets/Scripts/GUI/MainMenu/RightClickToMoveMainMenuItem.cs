using UnityEngine;
using System.Collections;

public class RightClickToMoveMainMenuItem : MouseControlMainMenuItem {
    public override void click() {
        // Debug.Log(this.GetType());
		base.click();
        controlsArray.switchControlTypeToRightClickToMove();
    }
}
