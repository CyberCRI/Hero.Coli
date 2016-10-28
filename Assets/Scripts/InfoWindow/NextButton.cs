using UnityEngine;
using System.Collections;

public class NextButton : MonoBehaviour {

  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("NextButton::OnPress()");
      InfoWindowManager.next();
    }
  }
}
