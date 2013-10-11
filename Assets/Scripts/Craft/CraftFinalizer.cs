using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftFinalizer : MonoBehaviour {
  public Inventory                      inventory;
  public CraftZoneManager               craftZoneManager;

  public FinalizationInfoPanelManager   finalizationInfoPanelManager;
  public CraftFinalizationButton        craftFinalizationButton;

  public static Dictionary<Inventory.AddingFailure, string>   statusMessagesDictionary =
    new Dictionary<Inventory.AddingFailure, string>() {
      {Inventory.AddingFailure.DEFAULT,         "invalid device!"},
      {Inventory.AddingFailure.NONE,            "new device"},
      {Inventory.AddingFailure.SAME_BRICKS,     "device with same bricks already exists!"},
      {Inventory.AddingFailure.SAME_NAME,       "device with same name already exists!"}
    };

  /*
  //promoter
  private static float testbeta = 10.0f;
  private static string testformula = "![0.8,2]LacI";
  //rbs
  private static float testrbsFactor = 1.0f;
  //gene
  private static string testproteinName = DevicesDisplayer.getRandomProteinName();
  //terminator
  private static float testterminatorFactor = 1.0f;

  private static Device getTestDevice() {
  
    string randomName = DevicesDisplayer.devicesNames[Random.Range (0, DevicesDisplayer.devicesNames.Count)];
    Device testDevice = Device.buildDevice(randomName, testbeta, testformula, testrbsFactor, testproteinName, testterminatorFactor);

    return testDevice;
  }
  */

  public void finalizeCraft() {
    //create new device from current biobricks in craft zone
    //TODO names for crafted elements???
    /*
    LinkedList<DisplayedBioBrick> dBricks = _craftZoneManager.getCurrentDisplayedBricks();
    LinkedList<BioBrick> bricks = new LinkedList<BioBrick>();
    foreach (DisplayedBioBrick dBrick in dBricks) {
      bricks.AddLast(dBrick._biobrick);
    }
    string randomName = DevicesDisplayer.devicesNames[Random.Range (0, DevicesDisplayer.devicesNames.Count)];
    ExpressionModule craftedModule = new ExpressionModule(randomName+"_module", bricks);
    LinkedList<ExpressionModule> craftedModules = new LinkedList<ExpressionModule>();
    craftedModules.AddLast(craftedModule);
    Device craftedDevice = Device.buildDevice(randomName, craftedModules);
    if(craftedDevice!=null){
      Logger.Log("CraftFinalizer::finalizeCraft() succeeded: craftedDevice="+craftedDevice);
      _inventory.askAddDevice(craftedDevice);
    } else {
      Logger.Log("CraftFinalizer::finalizeCraft() failed: craftedModules="+craftedModules);
    }
    */
    Device currentDevice = craftZoneManager.getCurrentDevice();
    if(currentDevice!=null){
      inventory.askAddDevice(currentDevice);
      Logger.Log("CraftFinalizer::finalizeCraft(): device="+currentDevice);
    } else {
      Logger.Log("CraftFinalizer::finalizeCraft() failed: invalid device (null)");
    }
  }

  public void setDisplayedDevice(Device device){
    Logger.Log("CraftFinalizer::setDisplayedDevice("+device+")", Logger.Level.DEBUG);
    finalizationInfoPanelManager.setDisplayedDevice(device, getDeviceStatus(device));
  }

  private string getDeviceStatus(Device device){
    Logger.Log("CraftFinalizer::getDeviceStatus("+device+")", Logger.Level.TRACE);
    if(device!=null) {
      Inventory.AddingFailure failure = inventory.canAddDevice(device);
      return statusMessagesDictionary[failure];
    } else {
      Logger.Log("CraftFinalizer::getDeviceStatus: invalid device", Logger.Level.WARN);
      return statusMessagesDictionary[Inventory.AddingFailure.DEFAULT];
    }
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
