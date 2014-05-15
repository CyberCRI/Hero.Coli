using UnityEngine;
using System.Collections;

public class ToEquipButton : MonoBehaviour {

	 private void OnPress(bool isPressed) {
	    if(isPressed) {
	      Logger.Log("ToEquipButton::OnPress()", Logger.Level.INFO);
	      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen2);
	    }
	  }
}
