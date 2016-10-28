using UnityEngine;
using System.Collections;

public class LeftClickToMoveButton : MonoBehaviour {
    
  private void OnPress(bool isPressed)
  {
    if(isPressed) {
      Logger.Log("LeftClickToMoveButton::OnPress()");
      ControlsMainMenuItemArray.get ().switchControlTypeToLeftClickToMove();
    }
  }
}
