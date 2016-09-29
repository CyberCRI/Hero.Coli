using UnityEngine;
using System.Collections;

public class ControlsMainMenuItem : SettingMainMenuItem {
    public override void click() {
        // Debug.Log(this.GetType());
		RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.CONTROLS.ToString()));
        MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.CONTROLS);
    }
}
