using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftFinalizer : MonoBehaviour {
  private CraftZoneManager               craftZoneManager;

  public FinalizationInfoPanelManager   finalizationInfoPanelManager;
  public CraftFinalizationButton        craftFinalizationButton;

  public static Dictionary<Inventory.AddingResult, string>   statusMessagesDictionary =
    new Dictionary<Inventory.AddingResult, string>() {
      {Inventory.AddingResult.SUCCESS,                "new device"},
      {Inventory.AddingResult.FAILURE_SAME_NAME,      "device with same name already exists!"},
      {Inventory.AddingResult.FAILURE_SAME_BRICKS,    "device with same bricks already exists!"},
      {Inventory.AddingResult.FAILURE_SAME_DEVICE,    ""},//a device with same name and same bricks already exists
      {Inventory.AddingResult.FAILURE_DEFAULT,        "invalid device!"}
    };

  public void finalizeCraft() {
    //create new device from current biobricks in craft zone
    Logger.Log("CraftFinalizer::finalizeCraft()", Logger.Level.TRACE);
    Device currentDevice = craftZoneManager.getCurrentDevice();
    if(currentDevice!=null){
      Inventory.AddingResult addingResult = Inventory.get().askAddDevice(currentDevice);
      if(addingResult == Inventory.AddingResult.SUCCESS) {
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
    Logger.Log("CraftFinalizer::setDisplayedDevice("+device+")", Logger.Level.TRACE);

    Inventory.AddingResult addingResult = Inventory.get().canAddDevice(device);
    Logger.Log("CraftFinalizer::setDisplayedDevice(): addingResult="+addingResult, Logger.Level.TRACE);
    string status = statusMessagesDictionary[addingResult];
    Logger.Log("CraftFinalizer::setDisplayedDevice(): status="+status, Logger.Level.TRACE);

    bool enabled = (addingResult == Inventory.AddingResult.SUCCESS);
    craftFinalizationButton.setEnabled(enabled);
    Logger.Log("CraftFinalizer::setDisplayedDevice(): "+craftFinalizationButton+".setEnabled("+enabled+")", Logger.Level.TRACE);
    finalizationInfoPanelManager.setDisplayedDevice(device, status);
    Logger.Log("CraftFinalizer::setDisplayedDevice(): finalizationInfoPanelManager.setDisplayedDevice(device, status)", Logger.Level.TRACE);
  }

  public void randomRename() {
    Logger.Log("CraftFinalizer::randomRename", Logger.Level.TRACE);
    Device currentDevice = craftZoneManager.getCurrentDevice();
    string newName = Inventory.get().getAvailableDeviceName();
    Device newDevice = Device.buildDevice(newName, currentDevice.getExpressionModules());
    if(newDevice != null){
      Logger.Log("CraftFinalizer::randomRename craftZoneManager.setDevice("+newDevice+")", Logger.Level.TRACE);
      craftZoneManager.setDevice(newDevice);
    } else {
      Logger.Log("CraftFinalizer::randomRename failed Device.buildDevice(name="+newName
        +", modules="+Logger.ToString<ExpressionModule>(currentDevice.getExpressionModules())+")", Logger.Level.WARN);
    }
  }

  void Start()
  {
    craftZoneManager = CraftZoneManager.get();
  }
}
