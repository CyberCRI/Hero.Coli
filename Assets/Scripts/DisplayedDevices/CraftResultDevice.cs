using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftResultDevice : DisplayedDevice {
  protected override void OnPress(bool isPressed)
  {
    base.OnPress(isPressed);
    Logger.Log("CraftResultDevice::OnPress("+isPressed+")", Logger.Level.INFO);
  }

  protected override void OnHover(bool isOver)
  {
    base.OnHover(isOver);
  }
}