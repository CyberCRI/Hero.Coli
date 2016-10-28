using UnityEngine;
using System.Collections;

public class AvailableGenesButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("AvailableGenesButton::OnPress()");
      AvailableBioBricksManager.get().displayGenes();
    }
  }
}
