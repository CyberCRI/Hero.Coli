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
      if(_device==null)
      {
        Logger.Log("InventoriedDisplayedDevice::OnPress _device==null", Logger.Level.WARN);
        return;
      }

			DeviceContainer.AddingResult addingResult = _devicesDisplayer.askAddEquipedDevice(_device);
      Logger.Log("InventoriedDisplayedDevice::OnPress() added device result="+addingResult+", "+getDebugInfos(), Logger.Level.ERROR);
      if(DeviceContainer.AddingResult.FAILURE_SAME_NAME == addingResult
         || DeviceContainer.AddingResult.FAILURE_SAME_DEVICE == addingResult)
      {
                Debug.LogError("InventoriedDisplayedDevice will askRemoveEquipedDevice("+_device+")");
          _devicesDisplayer.askRemoveEquipedDevice(_device);
      }

			//pointer Animation

			if(gameObject.transform.FindChild("tutorialArrow(Clone)"))
			{
				ArrowAnimation.Delete("InventoryDevicesSlotsPanel");
				GUITransitioner.get ().arrowManager.isInventoryAnimPlaying = false;
			}

		}
	}

}