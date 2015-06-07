using UnityEngine;
using System.Collections;

public class GoToOtherGameModeMainMenuItem : MainMenuItem {
    private string adventure = "MENU.ADVENTURE";
    private string sandbox = "MENU.SANDBOX";

    public override void initialize() {
        itemName = GameConfiguration.GameMode.ADVENTURE == MemoryManager.get ().configuration.getMode() ? sandbox : adventure;
    }

    public override void click() {
        Debug.LogWarning("the game will sandbox...");
        GameStateController.get ().goToOtherGameMode();
    }
}
