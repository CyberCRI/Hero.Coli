using UnityEngine;
using System.Collections;

public class AvailableRBSButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("AvailableRBSButton::OnPress()", Logger.Level.INFO);
      AvailableBioBricksManager.get().displayRBS();
    }
  }
}
