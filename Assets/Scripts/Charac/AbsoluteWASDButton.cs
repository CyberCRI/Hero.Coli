using UnityEngine;
using System.Collections;

public class AbsoluteWASDButton : MonoBehaviour {
    
  private void OnPress(bool isPressed)
  {
    if(isPressed) {
      Debug.Log(this.GetType() + " AbsoluteWASDButton::OnPress()");
      ControlsMainMenuItemArray.get ().switchControlTypeToAbsoluteWASD();
    }
  }
}
