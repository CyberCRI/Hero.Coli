using UnityEngine;
using System.Collections;

public class ToCraftButton : MonoBehaviour {

	public AvailableBioBricksManager _availableBioBricksManager;

  private void OnPress(bool isPressed)
  {
    if(isPressed && _availableBioBricksManager.getAvailableBioBricks().Count != 0) {
      Logger.Log("ToCraftButton::OnPress()", Logger.Level.INFO);
      GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen3);
    }
  }

  void Start()
  {
    _availableBioBricksManager = AvailableBioBricksManager.get();
  }
}