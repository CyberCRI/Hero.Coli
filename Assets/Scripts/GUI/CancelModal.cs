using UnityEngine;
using System.Collections;

public class CancelModal : ModalButton {
  protected override void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("CancelModal::OnPress()", Logger.Level.INFO);

      GameStateController.get().tryUnlockPause();
      
      ModalManager.unsetModal();
    }
  }
}
