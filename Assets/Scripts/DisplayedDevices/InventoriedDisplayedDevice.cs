using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class InventoriedDisplayedDevice : DisplayedDevice {
	
	protected override void OnPress(bool isPressed) {
		if(isPressed) {
			Debug.Log("InventoriedDisplayedDevice on press() "+getDebugInfos());
			if(_deviceType == DevicesDisplayer.DeviceType.Inventoried) {
				_devicesDisplayer.addEquipedDevice(_device);
			}
		}
	}

}