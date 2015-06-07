using UnityEngine;
using System.Collections;

public class RelativeWASDMainMenuItem : ControlMainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will relative wasd...");
        controlsArray.switchControlTypeToRelativeWASD();
    }
}
