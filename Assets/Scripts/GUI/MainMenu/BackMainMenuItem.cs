using UnityEngine;
using System.Collections;

public class BackMainMenuItem : MainMenuItem {
    public override void click() {
        Logger.Log("the game will back to main menu...");
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.DEFAULT);
    }
}
