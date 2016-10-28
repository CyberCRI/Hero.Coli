using UnityEngine;
using System.Collections;

public class AvailableGenesButton : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Debug.Log(this.GetType() + " AvailableGenesButton::OnPress()");
      AvailableBioBricksManager.get().displayGenes();
    }
  }
}
