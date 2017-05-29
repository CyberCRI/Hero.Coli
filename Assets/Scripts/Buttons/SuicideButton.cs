using UnityEngine;

public class SuicideButton : MonoBehaviour {

	private void OnPress(bool isPressed)
    {
        if(isPressed) {
            Character.get().kill(CustomDataValue.SUICIDEBUTTON);
        }   
    }
}
