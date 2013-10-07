using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class ListedDevice : DisplayedDevice {

  private static string _hoveredSuffix = "Hovered";
  private static string _pressedSuffix = "Pressed";
  protected new string _currentSpriteName = "bullet";

  protected override void OnPress(bool isPressed) {
    //TODO load clicked device into top craft screen
    Debug.Log("ListedDevice::OnPress("+isPressed+")");
    setPressed(isPressed);
  }

  protected void OnHover(bool isHovered) {
    Logger.Log("ListedDevice::OnHover("+isHovered+")", Logger.Level.TRACE);
    setHovered(isHovered);
  }


  private void setHovered(bool isHovered) {
    Logger.Log("ListedDevice::setHovered("+isHovered+")", Logger.Level.TRACE);
    if(isHovered) {
      setSprite(_currentSpriteName + _hoveredSuffix);
    } else {
      setSprite(_currentSpriteName);
    }
  }


  private void setPressed(bool isPressed) {
    Debug.Log("ListedDevice::setPressed("+isPressed+")");
    if(isPressed) {
      setSprite(_currentSpriteName + _pressedSuffix);
    } else {
      //managed by hover
      //setSprite(_currentSpriteName);
    }
  }
}