using UnityEngine;
using System.Collections;

public class LearnMoreOptionsMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will learnMoreOption...");
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.LEARNMORE.ToString()));
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.LEARNMOREOPTIONS);
    }
}
