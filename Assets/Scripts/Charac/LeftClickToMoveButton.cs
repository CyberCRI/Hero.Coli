using UnityEngine;
using System.Collections;

public class LeftClickToMoveButton : MonoBehaviour {
    
  private void OnPress(bool isPressed)
  {
    if(isPressed) {
      Debug.Log(this.GetType() + " OnPress()");
      ControlsMainMenuItemArray.get ().switchControlTypeToLeftClickToMove();
    }
  }
}
