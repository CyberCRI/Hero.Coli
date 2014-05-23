using UnityEngine;
using System.Collections;

public class LeftClickToMoveButton : CellControlButton {
    
  private void OnPress(bool isPressed)
  {
    if(isPressed && cellControl) {
      Logger.Log("LeftClickToMoveButton::OnPress()", Logger.Level.INFO);
      cellControl.switchControlTypeToLeftClickToMove();
    }
  }
}
