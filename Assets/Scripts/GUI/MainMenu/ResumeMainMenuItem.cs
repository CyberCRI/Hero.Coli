
public class ResumeMainMenuItem : MainMenuItem {
    public override void click() {
		base.click ();
        string label = isStart() ? CustomDataValue.START.ToString() : CustomDataValue.RESUME.ToString() ;
        GameStateController.get ().leaveMainMenu ();
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, label));
    }

    public bool isStart()
    {
        return itemName == MainMenuManager.newGameKey;
    }
}