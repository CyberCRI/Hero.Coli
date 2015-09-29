using UnityEngine;
using System.Collections;

public class LanguagesMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will language...");
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.LANGUAGE.ToString()));
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.LANGUAGES);
    }
}
