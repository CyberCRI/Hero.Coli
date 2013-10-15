using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftFinalizer : MonoBehaviour {
  public Inventory                      inventory;
  public CraftZoneManager               craftZoneManager;

  public FinalizationInfoPanelManager   finalizationInfoPanelManager;
  public CraftFinalizationButton        craftFinalizationButton;

  public static Dictionary<Inventory.AddingResult, string>   statusMessagesDictionary =
    new Dictionary<Inventory.AddingResult, string>() {
      {Inventory.AddingResult.SUCCESS,         "new device"},
      {Inventory.AddingResult.FAILURE_SAME_NAME,       "device with same name already exists!"},
      {Inventory.AddingResult.FAILURE_SAME_BRICKS,     "device with same bricks already exists!"},
      {Inventory.AddingResult.FAILURE_SAME_DEVICE,     ""},//a device with same name and same bricks already exists
      {Inventory.AddingResult.FAILURE_DEFAULT,         "invalid device!"}
    };

  public void finalizeCraft() {
    //create new device from current biobricks in craft zone
    Logger.Log("CraftFinalizer::finalizeCraft()", Logger.Level.TRACE);
    Device currentDevice = craftZoneManager.getCurrentDevice();
    if(currentDevice!=null){
      Inventory.AddingResult failure = inventory.askAddDevice(currentDevice);
      if(failure == Inventory.AddingResult.SUCCESS) {
        craftZoneManager.displayDevice();
        Logger.Log("CraftFinalizer::finalizeCraft(): device="+currentDevice, Logger.Level.TRACE);
      } else {
        Logger.Log("CraftFinalizer::finalizeCraft(): device="+currentDevice, Logger.Level.TRACE);
      }
    } else {
      Logger.Log("CraftFinalizer::finalizeCraft() failed: invalid device (null)", Logger.Level.TRACE);
    }
  }

  public void setDisplayedDevice(Device device){
    Logger.Log("CraftFinalizer::setDisplayedDevice("+device+")", Logger.Level.DEBUG);

    Inventory.AddingResult failure = inventory.canAddDevice(device);
    string status = statusMessagesDictionary[failure];

    craftFinalizationButton.SetActive(failure == Inventory.AddingResult.SUCCESS);
    finalizationInfoPanelManager.setDisplayedDevice(device, status);
  }

  public void randomRename() {
    Logger.Log("CraftFinalizer::randomRename", Logger.Level.DEBUG);
    Device currentDevice = craftZoneManager.getCurrentDevice();
    string newName = inventory.getAvailableDeviceName();
    Device newDevice = Device.buildDevice(newName, currentDevice.getExpressionModules());
    if(newDevice != null){
      Logger.Log("CraftFinalizer::randomRename craftZoneManager.setDevice("+newDevice+")", Logger.Level.TRACE);
      craftZoneManager.setDevice(newDevice);
    } else {
      Logger.Log("CraftFinalizer::randomRename failed Device.buildDevice(name="+newName
        +", modules="+Logger.ToString<ExpressionModule>(currentDevice.getExpressionModules())+")", Logger.Level.TRACE);
    }
  }
}
