using UnityEngine;
using System.Collections;

public class AvailableTerminatorsButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("AvailableTerminatorsButton::OnPress()", Logger.Level.INFO);
      AvailableBioBricksManager.get().displayTerminators();
    }
  }
}
