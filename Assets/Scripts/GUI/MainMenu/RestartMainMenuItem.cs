using UnityEngine;

public class RestartMainMenuItem : MainMenuItem {
    public override void click() {
        // Debug.Log(this.GetType());
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.RESTART.ToString()));
        GameStateController.restart ();
    }
}
