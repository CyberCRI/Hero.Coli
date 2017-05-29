using UnityEngine;
using System.Collections;

public class LeftClickToMoveMainMenuItem : MouseControlMainMenuItem {
    public override void click() {
        // Debug.Log(this.GetType());
		base.click();
        controlsArray.switchControlTypeToLeftClickToMove();
    }
}
