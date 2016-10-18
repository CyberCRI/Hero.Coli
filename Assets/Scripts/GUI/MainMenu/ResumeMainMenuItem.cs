
public class ResumeMainMenuItem : MainMenuItem {
    public override void click() {
        string label = isStart() ? CustomDataValue.START.ToString() : CustomDataValue.RESUME.ToString() ;
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, label));
        GameStateController.get ().leaveMainMenu ();
    }

    public bool isStart()
    {
        return itemName == MainMenuManager.newGameKey;
    }
}