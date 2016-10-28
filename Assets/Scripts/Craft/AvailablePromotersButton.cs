using UnityEngine;
using System.Collections;

public class AvailablePromotersButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      // Debug.Log(this.GetType() + " OnPress()");
      AvailableBioBricksManager.get().displayPromoters();
    }
  }
}
