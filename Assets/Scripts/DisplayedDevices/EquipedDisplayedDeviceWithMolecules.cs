using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipedDisplayedDeviceWithMolecules : DisplayedDevice {

  private static GameObject           equipedDevice;
  
  void OnEnable() {
    Logger.Log("DisplayedDeviceWithMolecules::OnEnable "+_device, Logger.Level.INFO);
  }
  
  void OnDisable() {
    Logger.Log("DisplayedDeviceWithMolecules::OnDisable "+_device, Logger.Level.INFO);
  }
  
  protected override void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("DisplayedDeviceWithMolecules::OnPress() "+getDebugInfos(), Logger.Level.INFO);
      if(_device == null)
      {
        Logger.Log("DisplayedDeviceWithMolecules::OnPress _device == null", Logger.Level.WARN);
        return;
      }
      if (_devicesDisplayer.IsEquipScreen()) {
        TooltipManager.displayTooltip();
        _devicesDisplayer.askRemoveEquipedDevice(_device);
      }
    }
  }
  
  void initIfNecessary() {
    Logger.Log("DisplayedDeviceWithMolecules::initIfNecessary starts", Logger.Level.WARN);
    if(
      (null == equipedDevice)
      ) 
    {
      equipedDevice = DevicesDisplayer.get().equipedDevice;      
    }
  }
  
  // Use this for initialization
  void Start () {
    Logger.Log("DisplayedDeviceWithMolecules::Start", Logger.Level.TRACE);
  }
}