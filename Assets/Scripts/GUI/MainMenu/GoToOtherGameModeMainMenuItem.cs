public class GoToOtherGameModeMainMenuItem : MainMenuItem
{
    private const string _adventure = "MENU.ADVENTURE";
    private const string _sandbox = "MENU.SANDBOX";

    public override void initialize()
    {
        itemName = GameConfiguration.GameMode.ADVENTURE == MemoryManager.get().configuration.getMode() ? _sandbox : _adventure;
    }

    public override void click()
    {
        // Debug.Log(this.GetType() + " click");
		base.click();
        CustomDataValue modeValue = GameConfiguration.GameMode.ADVENTURE == MemoryManager.get().configuration.getMode() ? CustomDataValue.SANDBOX : CustomDataValue.ADVENTURE;
        RedMetricsManager.get().sendRichEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, modeValue.ToString()));

        GameStateController.get().goToOtherGameMode();
    }
}
