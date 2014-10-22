using UnityEngine;
using System.Collections;

public class CancelModal : MonoBehaviour {
  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("CancelModal::OnPress()", Logger.Level.INFO);
      GameStateController.get().changeState(GameState.Game);
      GameStateController.get().dePauseForbidden = false;
      ModalManager.unsetModal();
    }
  }
}
