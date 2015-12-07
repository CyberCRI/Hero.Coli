using UnityEngine;
using System.Collections;

public class GoToOtherGameModeMainMenuItem : MainMenuItem {
    private string adventure = "MENU.ADVENTURE";
    private string sandbox = "MENU.SANDBOX";
    
    //public bool _isLocked = true;

    public override void initialize() {
        Debug.LogError("GoToOtherGameModeMainMenuItem::initialize");
        itemName = GameConfiguration.GameMode.ADVENTURE == MemoryManager.get ().configuration.getMode() ? sandbox : adventure;
        //if(_isLocked) {
        //    Debug.LogError("GoToOtherGameModeMainMenuItem::initialize displayed = false;");
        //    displayed = false;
        //}
        displayed = MemoryManager.get ().configuration.isLoggedIn();
    }

    public override void click() {
		Debug.LogWarning("the game will "+itemName+"...");

		CustomDataValue modeValue = GameConfiguration.GameMode.ADVENTURE == MemoryManager.get ().configuration.getMode() ? CustomDataValue.SANDBOX : CustomDataValue.ADVENTURE;
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, modeValue.ToString()));

        GameStateController.get ().goToOtherGameMode();
    }
}
