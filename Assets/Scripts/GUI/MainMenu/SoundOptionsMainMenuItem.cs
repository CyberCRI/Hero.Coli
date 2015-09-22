using UnityEngine;
using System.Collections;

public class SoundOptionsMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will soundOption...");
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.SOUNDOPTIONS);
    }
}
