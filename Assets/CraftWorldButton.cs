using UnityEngine;
using System.Collections;

public class CraftWorldButton : MonoBehaviour {

	 private void OnPress(bool isPressed) {
	    if(isPressed) {
	      Logger.Log("CraftWorldButton::OnPress()", Logger.Level.INFO);
	      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen1);
	    }
	  }
}
