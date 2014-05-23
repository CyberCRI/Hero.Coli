using UnityEngine;
using System.Collections;

public class RestartButton : MonoBehaviour {    
  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("RestartButton::OnPress()", Logger.Level.INFO);
      Application.LoadLevel("Master-Demo-0.1");
    }
  }
}
