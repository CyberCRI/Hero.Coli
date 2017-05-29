using UnityEngine;
using System.Collections;

public class ControlsMainMenuItem : MainMenuItem
{
    public override void click()
    {
        // Debug.Log(this.GetType());
		base.click();
        MainMenuManager.get().switchTo(MainMenuManager.MainMenuScreen.CONTROLS);
        RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.CONTROLS.ToString()));
    }
}
