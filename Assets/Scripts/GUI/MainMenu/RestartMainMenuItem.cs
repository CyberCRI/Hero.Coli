using UnityEngine;
using System.Collections;

public class RestartMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will restart...");
        GameStateController.restart ();
    }
}
