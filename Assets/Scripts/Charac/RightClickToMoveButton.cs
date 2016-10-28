using UnityEngine;
using System.Collections;

public class RightClickToMoveButton : MonoBehaviour {
    
  private void OnPress(bool isPressed)
  {
    if(isPressed) {
      Debug.Log(this.GetType() + " RightClickToMoveButton::OnPress()");
      ControlsMainMenuItemArray.get ().switchControlTypeToRightClickToMove();
    }
  }
}
