using UnityEngine;
using System.Collections;

public class ContributeMainMenuItem : MainMenuItem
{
    public override void click()
    {
        // Debug.Log(this.GetType() + " click");
        MainMenuManager.get().switchTo(MainMenuManager.MainMenuScreen.CONTRIBUTE);
        RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.CONTRIBUTE.ToString()));
    }
}
