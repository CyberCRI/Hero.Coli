using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class InventoriedDisplayedDevice : DisplayedDevice {
	new public DevicesDisplayer.DeviceType _deviceType = DevicesDisplayer.DeviceType.Inventoried;
	new public static Object prefab = Resources.Load("GUI/screen1/EquipedDevices/InventoriedDeviceButtonPrefab");
	
	protected override void OnPress(bool isPressed) {
		if(isPressed) {
			Debug.Log("on press() "+getDebugInfos());
			if(_deviceType == DevicesDisplayer.DeviceType.Inventoried) {
				_devicesDisplayer.addInventoriedDevice(_device);
			}
		}
	}
}