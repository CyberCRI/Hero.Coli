using UnityEngine;
using System.Collections;

public class QuitModalWindow : ModalButton {
  protected override void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("QuitModalWindow::OnPress()", Logger.Level.INFO);
      GameStateController.get().tryUnlockPause();
    }
    base.OnPress(isPressed);
  }
}
