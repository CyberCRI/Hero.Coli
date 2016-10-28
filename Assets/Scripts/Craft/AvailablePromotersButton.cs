using UnityEngine;
using System.Collections;

public class AvailablePromotersButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Debug.Log(this.GetType() + " AvailablePromotersButton::OnPress()");
      AvailableBioBricksManager.get().displayPromoters();
    }
  }
}
