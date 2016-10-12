using UnityEngine;

public class SuicideButton : MonoBehaviour {

	private void OnPress(bool isPressed)
    {
        if(isPressed) {
            RedMetricsManager.get ().sendEvent(TrackingEvent.DEATH, new CustomData(CustomDataTag.SOURCE, CustomDataValue.SUICIDEBUTTON.ToString()));
            Hero.get().kill();
        }   
    }
}
