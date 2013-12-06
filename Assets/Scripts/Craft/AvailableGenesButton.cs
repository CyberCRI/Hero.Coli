using UnityEngine;
using System.Collections;

public class AvailableGenesButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("AvailableGenesButton::OnPress()", Logger.Level.INFO);
      AvailableBioBricksManager.get().displayGenes();
    }
  }
}
