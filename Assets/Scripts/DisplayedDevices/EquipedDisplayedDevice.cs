using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class EquipedDisplayedDevice : DisplayedDevice {

	protected override void OnPress(bool isPressed) {
		if(isPressed) {
			Debug.Log("EquipedDisplayedDevice on press() "+getDebugInfos());
			if (_devicesDisplayer.IsEquipScreen()) {
				_devicesDisplayer.askRemoveEquipedDevice(_device);
			}
		}
	}

}