using UnityEngine;

public class ResetUserIdButton : MonoBehaviour {
	private void OnPress(bool isPressed)
    {
        if(isPressed) {
            RedMetricsManager.get ().sendEvent (TrackingEvent.SELECTMENU, new CustomData (CustomDataTag.OPTION, CustomDataValue.RESETCONFIGURATION.ToString()));
		    MemoryManager.get().configuration.reset();
        }   
    }
}