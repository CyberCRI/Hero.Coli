using UnityEngine;
using System.Collections;

public class ModalButton : MonoBehaviour {
  protected virtual void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("ModalButton::OnPress()", Logger.Level.INFO);
      ModalManager.get().unsetModal(this);
    }
  }
}
