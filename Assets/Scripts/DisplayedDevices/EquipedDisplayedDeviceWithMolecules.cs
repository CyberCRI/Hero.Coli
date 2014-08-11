using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipedDisplayedDeviceWithMolecules : MonoBehaviour {

  public GameObject equipedDeviceDummy;
  public GameObject equipedDevice;
  public Device device;
  public DisplayedDevice equipedDeviceScript;

  public void initialize()
  {
    Debug.LogError("EquipedDisplayedDeviceWithMolecules::initialize");
    setEquipedDevice();
  }

  public void setEquipedDevice()
  {
    Debug.LogError("EquipedDisplayedDeviceWithMolecules::setEquipedDevice");
    equipedDevice.transform.parent = transform;
    Debug.LogError("EquipedDisplayedDeviceWithMolecules::initialize parent");
    equipedDevice.transform.localPosition = equipedDeviceDummy.transform.localPosition;
    Debug.LogError("EquipedDisplayedDeviceWithMolecules::initialize localPosition");
    equipedDevice.transform.localScale = new Vector3(1f, 1f, 0);
    Debug.LogError("EquipedDisplayedDeviceWithMolecules::initialize localScale");
    equipedDevice.transform.localRotation = equipedDeviceDummy.transform.localRotation;
    Debug.LogError("EquipedDisplayedDeviceWithMolecules::initialize localRotation");
    equipedDeviceDummy.SetActive(false);
    Debug.LogError("EquipedDisplayedDeviceWithMolecules::initialize SetActive");
  }
  
  void OnEnable() {
    Logger.Log("EquipedDisplayedDeviceWithMolecules::OnEnable", Logger.Level.WARN);
  }
  
  void OnDisable() {
    Logger.Log("EquipedDisplayedDeviceWithMolecules::OnDisable", Logger.Level.WARN);
  }
  
  void OnPress(bool isPressed) {
    if(isPressed) {
      Logger.Log("EquipedDisplayedDeviceWithMolecules::OnPress() "+getDebugInfos(), Logger.Level.WARN);
      if(device == null)
      {
        Logger.Log("EquipedDisplayedDeviceWithMolecules::OnPress _device == null", Logger.Level.WARN);
        return;
      }
    }
  }
  
  // Use this for initialization
  void Start () {
    Logger.Log("EquipedDisplayedDeviceWithMolecules::Start", Logger.Level.WARN);
  }
    
  protected string getDebugInfos() {
        return "EquipedDisplayedDeviceWithMolecules inner device="+device+", inner equipedDeviceScript type="+equipedDeviceScript+", time="+Time.realtimeSinceStartup;
  }
}