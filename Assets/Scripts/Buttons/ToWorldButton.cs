using UnityEngine;
using System.Collections;

public class ToWorldButton : MonoBehaviour {

	 private void OnPress(bool isPressed) {
	    if(isPressed) {

	      Logger.Log("ToWorldButton::OnPress() actual screen: "+GUITransitioner.get()._currentScreen, Logger.Level.INFO);
	      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen1);
	    }
	  }
}
