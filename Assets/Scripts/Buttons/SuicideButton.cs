using UnityEngine;

public class SuicideButton : MonoBehaviour {

	private void OnPress(bool isPressed)
    {
        if(isPressed) {
            Hero.get().kill();
        }   
    }
}
