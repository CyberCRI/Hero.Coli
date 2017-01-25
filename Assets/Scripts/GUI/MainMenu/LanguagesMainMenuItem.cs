using UnityEngine;
using System.Collections;

public class LanguagesMainMenuItem : SettingMainMenuItem {
    public override void click() {
        // Debug.Log(this.GetType());
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.LANGUAGES);
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.LANGUAGE.ToString()));
    }
}
