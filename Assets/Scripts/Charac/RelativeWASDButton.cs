using UnityEngine;
using System.Collections;

public class RelativeWASDButton : CellControlButton {
    
  private void OnPress(bool isPressed)
  {
    if(isPressed && cellControl) {
      Logger.Log("RelativeWASDButton::OnPress()", Logger.Level.INFO);
      cellControl.switchControlTypeToRelativeWASD();
    }
  }
}
