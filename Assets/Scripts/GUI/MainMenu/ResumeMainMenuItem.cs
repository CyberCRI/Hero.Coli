using UnityEngine;
using System.Collections;

public class ResumeMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will resume...");
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.RESUME.ToString()));
        GameStateController.get ().leaveMainMenu ();
    }
}