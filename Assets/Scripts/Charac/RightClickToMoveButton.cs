using UnityEngine;
using System.Collections;

public class RightClickToMoveButton : CellControlButton {
    
  private void OnPress(bool isPressed)
  {
    if(isPressed && cellControl) {
      Logger.Log("RightClickToMoveButton::OnPress()", Logger.Level.INFO);
      cellControl.switchControlTypeToRightClickToMove();
    }
  }
}
