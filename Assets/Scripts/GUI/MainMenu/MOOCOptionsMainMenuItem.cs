using UnityEngine;
using System.Collections;

public class MOOCOptionsMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will moocOption...");
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.MOOCOPTIONS);
    }
}
