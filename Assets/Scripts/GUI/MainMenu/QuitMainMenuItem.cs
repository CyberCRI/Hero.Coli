using UnityEngine;
using System.Collections;

public class QuitMainMenuItem : MainMenuItem
{
    public override void click()
    {
        // Debug.Log(this.GetType());
		base.click();
		MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.QUIT);
        RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.QUIT.ToString()));
    }
}