using UnityEngine;

public class RestartMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will restart...");
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.RESTART.ToString()));
        GameStateController.restart ();
    }
}
