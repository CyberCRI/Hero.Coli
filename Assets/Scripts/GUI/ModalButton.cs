using UnityEngine;
using System.Collections;

public class ModalButton : MonoBehaviour {
  protected virtual void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("ModalButton::OnPress()", Logger.Level.INFO);
      ModalManager.unsetModal();
    }
  }
    
  public virtual void press()
  {
    Logger.Log("ModalButton::press()", Logger.Level.INFO);
    OnPress(true);
  }
}
