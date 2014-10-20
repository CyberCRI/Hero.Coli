using UnityEngine;
using System.Collections;

public class QuitModalWindow : ModalButton {
  protected override void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("ModalManager::OnPress()", Logger.Level.INFO);
      ModalManager.get().unsetModal();
    }
    base.OnPress(isPressed);
  }
}
