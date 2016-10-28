using UnityEngine;
using System.Collections;

public class NextButton : MonoBehaviour {

  void OnPress(bool isPressed) {
    if(isPressed) {
      Debug.Log(this.GetType() + " OnPress()");
      InfoWindowManager.next();
    }
  }
}
