using UnityEngine;
using System.Collections;

//TODO make better name (cf ToCraftButton & ToWorlButton)
public class ToEquipButton : MonoBehaviour {

	 private void OnPress(bool isPressed) {
	    if(isPressed) {
	      Logger.Log("ToEquipButton::OnPress()");
	      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen2);
	    }
	  }
}
