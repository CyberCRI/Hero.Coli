using UnityEngine;

public class RestartMainMenuItem : MainMenuItem {
    public override void click() {
        // Debug.Log(this.GetType());
		base.click();
		RedMetricsManager.get().sendRichEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.RESTART.ToString()));
        GameStateController.restart ();
    }
}
