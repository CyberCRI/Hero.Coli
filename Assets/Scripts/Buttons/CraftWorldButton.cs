using UnityEngine;
using System.Collections;

public class CraftWorldButton : MonoBehaviour {

	 private void OnPress(bool isPressed) {
	    if(isPressed) {
	      Debug.Log(this.GetType() + " CraftWorldButton::OnPress()");
	      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen1);
	    }
	  }
}
