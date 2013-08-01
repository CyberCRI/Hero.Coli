using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class EquipedDisplayedDevice : DisplayedDevice {
	new public DevicesDisplayer.DeviceType _deviceType = DevicesDisplayer.DeviceType.Equiped;	
	new public static Object prefab = Resources.Load("GUI/screen1/EquipedDevices/EquipedDeviceButtonPrefab");
	
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