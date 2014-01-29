using UnityEngine;
using System.Collections;

public class ToCraftButton : MonoBehaviour {

	public AvailableBioBricksManager biobrick;

private void OnPress(bool isPressed) {
	    if(isPressed && biobrick.getAvailableBioBricks().Count != 0) {
	      Logger.Log("ToCraftButton::OnPress()", Logger.Level.INFO);
	      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen3);
	    }
	  }
}