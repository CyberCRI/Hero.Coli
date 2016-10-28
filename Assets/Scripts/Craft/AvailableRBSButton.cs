using UnityEngine;
using System.Collections;

public class AvailableRBSButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("AvailableRBSButton::OnPress()");
      AvailableBioBricksManager.get().displayRBS();
    }
  }
}
