using UnityEngine;
using System.Collections;

public class ScienceMainMenuItem : MainMenuItem
{
    public override void click()
    {
        // Debug.Log(this.GetType());
        MainMenuManager.get().switchTo(MainMenuManager.MainMenuScreen.SCIENCE);
        RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.SCIENCE.ToString()));
    }
}