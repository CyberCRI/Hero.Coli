using UnityEngine;
using System.Collections;

public class SettingsBackMainMenuItem : MainMenuItem {
    public override void click() {
        // Debug.Log(this.GetType());
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.SETTINGS);
    }
}