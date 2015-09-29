using UnityEngine;
using System.Collections;

public class SoundOptionsMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will soundOption...");
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.SOUND.ToString()));
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.SOUNDOPTIONS);
    }
}
