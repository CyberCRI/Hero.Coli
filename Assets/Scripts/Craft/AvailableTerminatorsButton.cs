using UnityEngine;
using System.Collections;

public class AvailableTerminatorsButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Debug.Log(this.GetType() + " AvailableTerminatorsButton::OnPress()");
      AvailableBioBricksManager.get().displayTerminators();
    }
  }
}
