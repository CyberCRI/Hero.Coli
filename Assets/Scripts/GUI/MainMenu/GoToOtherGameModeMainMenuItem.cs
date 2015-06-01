using UnityEngine;
using System.Collections;

public class GoToOtherGameModeMainMenuItem : MainMenuItem {
    private string adventure = "MENU.ADVENTURE";
    private string sandbox = "MENU.SANDBOX";

    public override void initialize() {
        string labelKey = sandbox;
        string currentLevelCode = null;
        if(MemoryManager.get ().tryGetData(GameStateController._currentLevelKey, out currentLevelCode) && !string.IsNullOrEmpty(currentLevelCode))
        {
            if(GameStateController._adventureLevel1 != currentLevelCode) {
                labelKey = adventure;
            }
        }
        itemName = labelKey;
    }

    public override void click() {
        Debug.LogWarning("the game will sandbox...");
        GameStateController.get ().goToOtherGameMode();
    }
}
