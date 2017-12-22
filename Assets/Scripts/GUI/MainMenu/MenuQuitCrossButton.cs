using UnityEngine;

public class MenuQuitCrossButton : MainMenuItem {
#if ARCADE
    public override void initialize()
    {
        gameObject.SetActive(false);
    }
#endif
    public override void click()
    {
        // Debug.Log(this.GetType());
		base.click();
		MainMenuManager.get ().switchTo (MainMenuManager.MainMenuScreen.QUIT);
        RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.QUITCROSSMENU.ToString()));
    }
}
