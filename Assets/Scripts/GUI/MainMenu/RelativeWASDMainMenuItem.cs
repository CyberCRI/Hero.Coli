using UnityEngine;
using System.Collections;

public class RelativeWASDMainMenuItem : ControlMainMenuItem {
    public override void click() {
        // Debug.Log(this.GetType());
        controlsArray.switchControlTypeToRelativeWASD();
    }
}
