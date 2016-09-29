public class SettingsMainMenuItem : MainMenuItem {
    public override void click() {
        // Debug.Log(this.GetType());
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.SETTINGS.ToString()));
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.SETTINGS);
    }
}
