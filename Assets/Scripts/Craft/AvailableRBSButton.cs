using UnityEngine;
using System.Collections;

public class AvailableRBSButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Debug.Log(this.GetType() + " OnPress()");
      AvailableBioBricksManager.get().displayRBS();
    }
  }
}
