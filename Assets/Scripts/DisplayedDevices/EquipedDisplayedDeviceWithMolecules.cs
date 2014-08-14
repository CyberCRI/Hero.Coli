using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipedDisplayedDeviceWithMolecules : MonoBehaviour {
    
  public UILabel           namesLabel;
  public UILabel           valuesLabel;
  public GameObject equipedDeviceDummy;
  public GameObject equipedDevice;
  public Device device;
  public EquipedDisplayedDevice equipedDeviceScript;

  private DisplayedMolecule _displayedMolecule;

  public void initialize()
  {
    setEquipedDevice();
  }

  public void setEquipedDevice()
  {
    equipedDevice.transform.parent = transform;
    equipedDevice.transform.localPosition = equipedDeviceDummy.transform.localPosition;
    equipedDevice.transform.localScale = new Vector3(1f, 1f, 0);
    equipedDevice.transform.localRotation = equipedDeviceDummy.transform.localRotation;
    equipedDeviceDummy.SetActive(false);
    equipedDeviceScript.setDisplayBricks(false);
  }

  //TODO allow multiple protein management
  public void addDisplayedMolecule(DisplayedMolecule molecule)
  {
    _displayedMolecule = molecule;
    molecule.setDisplayType(DisplayedMolecule.DisplayType.DEVICEMOLECULELIST);
  }

  //TODO implement & allow multiple protein management
  public void removeDisplayedMolecule(string molecule)
  {
    Logger.Log("EquipedDisplayedDeviceWithMolecules::removedDisplayedMolecule not implemented", Logger.Level.WARN);
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
    namesLabel.text = "";
    valuesLabel.text = "";
  }

  void Update()
  {
    if(null != _displayedMolecule)
    {
      namesLabel.text = _displayedMolecule.getRealName();
      valuesLabel.text = _displayedMolecule.getVal();
    }
  }

  public void releaseMoleculeDisplay()
  {
    if(null != _displayedMolecule)
    {
      _displayedMolecule.setDisplayType(DisplayedMolecule.DisplayType.MOLECULELIST);
    }
  }

  protected string getDebugInfos() {
        return "EquipedDisplayedDeviceWithMolecules inner device="+device+", inner equipedDeviceScript type="+equipedDeviceScript+", time="+Time.realtimeSinceStartup;
  }
}