using UnityEngine;
using System.Collections;

public class AvailablePromotersButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("AvailablePromotersButton::OnPress()", Logger.Level.INFO);
      AvailableBioBricksManager.get().displayPromoters();
    }
  }
}
