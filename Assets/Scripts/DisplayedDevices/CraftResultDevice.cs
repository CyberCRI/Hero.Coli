using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftResultDevice : DisplayedDevice {
  protected override void OnPress(bool isPressed)
  {
    base.OnPress(isPressed);
    Logger.Log("CraftResultDevice::OnPress("+isPressed+")", Logger.Level.TEMP);
  }

  protected override void OnHover(bool isOver)
  {
    base.OnHover(isOver);
    Logger.Log("CraftResultDevice::OnHover", Logger.Level.TEMP);
  }
}