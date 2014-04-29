using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftFinalizer : MonoBehaviour {
  private CraftZoneManager               craftZoneManager;

	public CraftZoneManager TOCraftZoneManager {
			get {
				return this.craftZoneManager;
			}
			set {
				craftZoneManager = value;
			}
		}
  public FinalizationInfoPanelManager   finalizationInfoPanelManager;
  public CraftFinalizationButton        craftFinalizationButton;

  public static Dictionary<Inventory.AddingResult, string>   statusMessagesDictionary =
    new Dictionary<Inventory.AddingResult, string>() {
      {Inventory.AddingResult.SUCCESS,                "You found the recipe of a new device!"},
      {Inventory.AddingResult.FAILURE_SAME_NAME,      "There already is a device with the same name."},
      {Inventory.AddingResult.FAILURE_SAME_BRICKS,    "This recipe is already known."},
      {Inventory.AddingResult.FAILURE_SAME_DEVICE,    "You already know this recipe. There's nothing new to craft!"},
      {Inventory.AddingResult.FAILURE_DEFAULT,        "You will find here the result of the crafting operation."}
    };

  public void finalizeCraft() {
    //create new device from current biobricks in craft zone
    Logger.Log("CraftFinalizer::finalizeCraft()", Logger.Level.DEBUG);
    Device currentDevice = craftZoneManager.getCurrentDevice();
    if(currentDevice!=null){
      Inventory.AddingResult addingResult = Inventory.get().askAddDevice(currentDevice);
      if(addingResult == Inventory.AddingResult.SUCCESS) {
        craftZoneManager.displayDevice();
      }
      Logger.Log("CraftFinalizer::finalizeCraft(): device="+currentDevice, Logger.Level.TRACE);
    } else {
      Logger.Log("CraftFinalizer::finalizeCraft() failed: invalid device (null)", Logger.Level.WARN);
    }
  }

  public void setDisplayedDevice(Device device){
    Logger.Log("CraftFinalizer::setDisplayedDevice("+device+")", Logger.Level.TRACE);

    Inventory.AddingResult addingResult = Inventory.get().canAddDevice(device);
    string status = statusMessagesDictionary[addingResult];
    Logger.Log("CraftFinalizer::setDisplayedDevice(): addingResult="+addingResult+", status="+status, Logger.Level.TRACE);

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
