public class SettingsMainMenuItem : MainMenuItem
{
    public override void click()
    {
        // Debug.Log(this.GetType());
        MainMenuManager.get().switchTo(MainMenuManager.MainMenuScreen.SETTINGS);
        RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.SETTINGS.ToString()));
    }
}
