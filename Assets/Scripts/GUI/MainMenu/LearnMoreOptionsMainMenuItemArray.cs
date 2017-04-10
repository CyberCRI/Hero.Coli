using UnityEngine;

public class LearnMoreOptionsMainMenuItemArray : MainMenuItemArray
{
    private const string _learnMoreKeyPrefix = "MENU.LEARNMORE.";
    private const string _sameTabKey = _learnMoreKeyPrefix + "GOTOSAMETAB";
    private const string _newTabKey = _learnMoreKeyPrefix + "GOTONEWTAB";
    private const string _browserKey = _learnMoreKeyPrefix + "GOTOBROWSER";

    public void goToMOOC(string urlKey, bool newTab)
    {
        RedMetricsManager.get().sendEvent(TrackingEvent.GOTOMOOC);
        URLOpener.open(urlKey, newTab);
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
#if UNITY_WEBPLAYER
        setTabsMode(true);
#else
        setTabsMode(false);
#endif  
    }

    // will prepare two alternatives if true: same tab or new tab to access MOOC pages
    // otherwise, the default browser will open the MOOC page
    private void setTabsMode(bool isTabsMode)
    {
        // Debug.Log(this.GetType() + " setTabsMode " + isTabsMode);
        if (isTabsMode)
        {
            MainMenuManager.setVisibility(this, _sameTabKey, true, true, "setTabsMode");
            //rename "open in browser tab" into "new tab"
            MainMenuManager.replaceTextBy(_browserKey, _newTabKey, this, true, "setTabsMode");
        }
        else
        {
            //hide "same tab" option
            MainMenuManager.setVisibility(this, _sameTabKey, false, true, "setTabsMode");
            //rename "new tab" into "open in browser"
            MainMenuManager.replaceTextBy(_newTabKey, _browserKey, this, true, "setTabsMode");
        }
    }
}
