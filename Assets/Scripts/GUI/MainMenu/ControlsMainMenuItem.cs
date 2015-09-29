using UnityEngine;
using System.Collections;

public class ControlsMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will control...");
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.CONTROLS.ToString()));
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.CONTROLS);
    }
}
