using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class EquipedDisplayedDevice : DisplayedDevice {

  private static string               _activeSuffix = "Active";

  public bool                         _isActive;


	protected override void OnPress(bool isPressed) {
		if(isPressed) {
			Logger.Log("EquipedDisplayedDevice::OnPress() "+getDebugInfos(), Logger.Level.INFO);
			if (_devicesDisplayer.IsEquipScreen()) {
				_devicesDisplayer.askRemoveEquipedDevice(_device);
			}
		}
	}

  public void setActivity(bool activity) {
    _isActive = activity;
    if(activity) {
      setActive();
    } else {
      setInactive();
    }
  }

 public void setActive() {
   Logger.Log("EquipedDisplayedDevice::setActive", Logger.Level.TRACE);
   _isActive = true;
   setSprite(_currentSpriteName + _activeSuffix);
 }
 
 public void setInactive() {
   Logger.Log("EquipedDisplayedDevice::setInactive", Logger.Level.TRACE);
   _isActive = false;
   setSprite(_currentSpriteName);
 }

  // Use this for initialization
  void Start () {
    setActive();
  }

}