using UnityEngine;
using System.Collections;

public class QuitMainMenuItem : MainMenuItem
{
    public override void click()
    {
        // Debug.Log(this.GetType());
		base.click();
        RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.QUIT.ToString()));
        Application.Quit();
    }
}