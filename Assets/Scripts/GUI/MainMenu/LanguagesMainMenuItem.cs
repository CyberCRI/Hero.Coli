using UnityEngine;
using System.Collections;

public class LanguagesMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will language...");
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.LANGUAGES);
    }
}
