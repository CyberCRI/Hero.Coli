using UnityEngine;
using System.Collections;

public class RelativeWASDButton : MonoBehaviour {
    
  private void OnPress(bool isPressed)
  {
    if(isPressed) {
      Debug.Log(this.GetType() + " RelativeWASDButton::OnPress()");
      ControlsMainMenuItemArray.get ().switchControlTypeToRelativeWASD();
    }
  }
}
