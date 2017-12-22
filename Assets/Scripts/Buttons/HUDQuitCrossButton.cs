using UnityEngine;

public class HUDQuitCrossButton : MonoBehaviour {
#if ARCADE
    void Start()
    {
        gameObject.SetActive(false);
    }
#endif
	private void OnPress(bool isPressed)
    {
        if(isPressed) {
            RedMetricsManager.get ().sendEvent (TrackingEvent.SELECTMENU, new CustomData (CustomDataTag.OPTION, CustomDataValue.QUITCROSSHUD.ToString()));
            GameStateController.get ().goToMainMenu(MainMenuManager.MainMenuScreen.QUIT);
        }   
    }
}
