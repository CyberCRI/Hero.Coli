public class GraphicsOptionsMainMenuItem : MainMenuItem
{
    public override void click()
    {
        // Debug.Log(this.GetType());
		base.click();
        MainMenuManager.get().switchTo(MainMenuManager.MainMenuScreen.GRAPHICSOPTIONS);
        RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.GRAPHICS.ToString()));
    }
}
