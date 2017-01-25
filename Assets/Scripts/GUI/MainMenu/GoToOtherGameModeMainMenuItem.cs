using UnityEngine;
using System.Collections;

public class GoToOtherGameModeMainMenuItem : MainMenuItem {
    private string adventure = "MENU.ADVENTURE";
    private string sandbox = "MENU.SANDBOX";

    public override void initialize() {
        itemName = GameConfiguration.GameMode.ADVENTURE == MemoryManager.get ().configuration.getMode() ? sandbox : adventure;
    }

    public override void click() {
		// Debug.Log(this.GetType());

		CustomDataValue modeValue = GameConfiguration.GameMode.ADVENTURE == MemoryManager.get ().configuration.getMode() ? CustomDataValue.SANDBOX : CustomDataValue.ADVENTURE;
		RedMetricsManager.get().sendRichEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, modeValue.ToString()));

        GameStateController.get ().goToOtherGameMode();
    }
}
