using UnityEngine;
using System.Collections;

public class AvailableRBSButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Debug.Log(this.GetType() + " AvailableRBSButton::OnPress()");
      AvailableBioBricksManager.get().displayRBS();
    }
  }
}
