using UnityEngine;
using System.Collections;

public class ModalRestart : ModalButton {
  protected override void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("ModalRestart::OnPress()", Logger.Level.INFO);
      GameStateController.restart();
    }
    //FIXME usefulness?
    base.OnPress(isPressed);
  }
    
  public void press()
  {
    Logger.Log("ModalRestart::press()", Logger.Level.INFO);
    OnPress(true);
  }
}
