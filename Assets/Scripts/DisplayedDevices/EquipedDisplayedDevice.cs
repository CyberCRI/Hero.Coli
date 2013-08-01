using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class EquipedDisplayedDevice : DisplayedDevice {

	protected override void OnPress(bool isPressed) {
		if(isPressed) {
			Debug.Log("on press() "+getDebugInfos());
			if (_devicesDisplayer.IsEquipScreen()) {
				//_devicesDisplayer.removeDevice(_deviceID);
				_devicesDisplayer.removeDevice(DevicesDisplayer.DeviceType.Equiped, _device);
			}
		}
	}

}