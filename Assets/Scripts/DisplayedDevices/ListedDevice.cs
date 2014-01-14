using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class ListedDevice : DisplayedDevice {
 protected override void OnPress(bool isPressed) {
    Logger.Log("ListedDevice::OnPress("+isPressed+")", Logger.Level.INFO);
    setPressed(isPressed);
    if(isPressed) {
      //ask craft zone to display biobricks associated to this device
      CraftZoneManager.get().setDevice(_device);
    }
  }

  private void setPressed(bool isPressed) {
    Logger.Log("ListedDevice::setPressed("+isPressed+")", Logger.Level.TRACE);
    if(isPressed) {
      //setSprite(_currentSpriteName + _pressedSuffix);
    } else {
      //managed by hover
      //setSprite(_currentSpriteName);
    }
  }

  protected override void OnHover(bool isOver)
  {
    base.OnHover(isOver);
  }
}