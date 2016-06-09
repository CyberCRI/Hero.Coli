using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoriedDisplayedDevice : DisplayedDevice {
	
	void OnEnable() {
		Logger.Log("InventoriedDisplayedDevice::OnEnable "+_device, Logger.Level.TRACE);
	}
	
	protected override void OnPress(bool isPressed) {
		if(isPressed) {
			Logger.Log("InventoriedDisplayedDevice::OnPress() "+getDebugInfos(), Logger.Level.INFO);
      
            toggleEquiped();

			//pointer Animation

			if(gameObject.transform.FindChild("tutorialArrow(Clone)"))
			{
				ArrowAnimation.Delete("InventoryDevicesSlotsPanel");
				GUITransitioner.get ().arrowManager.isInventoryAnimPlaying = false;
			}

		}
	}

}