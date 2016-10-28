using UnityEngine;
using System.Collections;

//TODO make better name (cf ToCraftButton & ToWorlButton)
public class ToEquipButton : MonoBehaviour {

	 private void OnPress(bool isPressed) {
	    if(isPressed) {
	      // Debug.Log(this.GetType() + " OnPress()");
	      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen2);
	    }
	  }
}
