using UnityEngine;
using System.Collections;

public class AbsoluteWASDMainMenuItem : ControlMainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will absolute wasd...");
        controlsArray.switchControlTypeToAbsoluteWASD();
    }
}
