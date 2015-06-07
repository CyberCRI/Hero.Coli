using UnityEngine;
using System.Collections;

public class LeftClickToMoveButton : MonoBehaviour {
    
  private void OnPress(bool isPressed)
  {
    if(isPressed) {
      Logger.Log("LeftClickToMoveButton::OnPress()", Logger.Level.INFO);
      ControlsMainMenuItemArray.get ().switchControlTypeToLeftClickToMove();
    }
  }
}
