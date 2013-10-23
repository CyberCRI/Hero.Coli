using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class InventoriedDisplayedDevice : DisplayedDevice {
	
	protected override void OnPress(bool isPressed) {
		if(isPressed) {
			Logger.Log("InventoriedDisplayedDevice::OnPress() "+getDebugInfos(), Logger.Level.INFO);
			DeviceContainer.AddingResult addingResult = _devicesDisplayer.askAddEquipedDevice(_device);
      Logger.Log("InventoriedDisplayedDevice::OnPress() added device result="+addingResult+", "+getDebugInfos(), Logger.Level.INFO);
		}
	}

}