using UnityEngine;
using System.Collections;

public class AvailableTerminatorsButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("AvailableTerminatorsButton::OnPress()");
      AvailableBioBricksManager.get().displayTerminators();
    }
  }
}
