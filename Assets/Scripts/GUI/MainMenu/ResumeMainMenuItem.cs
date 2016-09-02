
public class ResumeMainMenuItem : MainMenuItem {
    public override void click() {
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.RESUME.ToString()));
        GameStateController.get ().leaveMainMenu ();
    }
}