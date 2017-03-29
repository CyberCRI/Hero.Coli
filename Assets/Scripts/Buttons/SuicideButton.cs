using UnityEngine;

public class SuicideButton : MonoBehaviour {

	private void OnPress(bool isPressed)
    {
        if(isPressed) {
            Character.get().kill(new CustomData(CustomDataTag.SOURCE, CustomDataValue.SUICIDEBUTTON.ToString()));
        }   
    }
}
