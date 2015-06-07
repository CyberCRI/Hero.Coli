using UnityEngine;
using System.Collections;

public class ControlsMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will control...");
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.CONTROLS);
    }
}
