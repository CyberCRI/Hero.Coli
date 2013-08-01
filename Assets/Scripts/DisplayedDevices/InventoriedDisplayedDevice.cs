using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class InventoriedDisplayedDevice : DisplayedDevice {
	public DevicesDisplayer.DeviceType _deviceType = DevicesDisplayer.DeviceType.Inventoried;
	public static Object prefab = Resources.Load("GUI/screen1/EquipedDevices/InventoriedDeviceButtonPrefab");
	
	protected override void OnPress(bool isPressed) {
		if(isPressed) {
			Debug.Log("on press() "+getDebugInfos());
			if(_deviceType == DevicesDisplayer.DeviceType.Inventoried) {
				_devicesDisplayer.addDevice(0, _deviceInfo, DevicesDisplayer.DeviceType.Equiped);
			}
		}
	}
}