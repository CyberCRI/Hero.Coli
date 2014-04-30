using UnityEngine;
using System.Collections;

public class ToWorldButton : MonoBehaviour {

	 private void OnPress(bool isPressed) {
	    if(isPressed) {

	      Logger.Log("ToWorldButton::OnPress()===>acutal screen ::"+GUITransitioner.get()._currentScreen, Logger.Level.INFO);
	      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen1);
	    }
	  }
}
