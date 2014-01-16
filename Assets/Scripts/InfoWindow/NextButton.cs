using UnityEngine;
using System.Collections;

public class NextButton : MonoBehaviour {
  public GameObject OKPanel;
  public GameStateController gameStateController;

  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("NextButton::OnPress()", Logger.Level.INFO);
      InfoWindowManager.next();
    }
  }
}
