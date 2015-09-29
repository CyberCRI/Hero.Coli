using UnityEngine;
using System.Collections;

public class MOOCOptionsMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will moocOption...");
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.LEARNMORE.ToString()));
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.MOOCOPTIONS);
    }
}
