using UnityEngine;
using System.Collections;

public class RightClickToMoveButton : MonoBehaviour {
    
  private void OnPress(bool isPressed)
  {
    if(isPressed) {
      Logger.Log("RightClickToMoveButton::OnPress()");
      ControlsMainMenuItemArray.get ().switchControlTypeToRightClickToMove();
    }
  }
}
