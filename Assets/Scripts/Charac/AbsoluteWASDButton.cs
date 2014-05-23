using UnityEngine;
using System.Collections;

public class AbsoluteWASDButton : CellControlButton {
    
  private void OnPress(bool isPressed)
  {
    if(isPressed && cellControl) {
      Logger.Log("AbsoluteWASDButton::OnPress()", Logger.Level.INFO);
      cellControl.switchControlTypeToAbsoluteWASD();
    }
  }
}
