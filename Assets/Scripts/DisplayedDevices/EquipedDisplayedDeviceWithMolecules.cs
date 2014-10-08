using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipedDisplayedDeviceWithMolecules : MonoBehaviour {
    
  public UILabel           namesLabel;
  public UILabel           valuesLabel;

  private GameObject _equipmentDeviceDummy;
  public GameObject equipmentDeviceDummy;
  public GameObject equipmentDevice;
  public EquipmentDevice equipmentDeviceScript;
    
  private GameObject _equipedDeviceDummy;
  public GameObject equipedDevice;
  public EquipedDisplayedDevice equipedDeviceScript;
    
  public Device device; 
  
    
  private DisplayedMolecule _displayedMolecule;

  public void initialize(GameObject staticEquipmentDeviceDummy, GameObject staticEquipedDeviceDummy)
  {
    if(null != equipmentDeviceDummy)
    {
        equipmentDeviceDummy.SetActive(false);
    }

        if(null == staticEquipmentDeviceDummy || null == staticEquipedDeviceDummy)
        {
      Logger.Log("EquipedDisplayedDeviceWithMolecules::initialize has null parameter", Logger.Level.WARN);
    }
    else
    {
      _equipmentDeviceDummy = staticEquipmentDeviceDummy;
            _equipedDeviceDummy = staticEquipedDeviceDummy;
      setEquipmentDevice();
    }
  }

  public void setEquipmentDevice()
  {
    equipmentDevice.transform.parent = transform;
    equipmentDevice.transform.localPosition = _equipmentDeviceDummy.transform.localPosition;
    equipmentDevice.transform.localScale = new Vector3(1f, 1f, 0);
    equipmentDevice.transform.localRotation = _equipmentDeviceDummy.transform.localRotation;    

    setEquipedDevice();
  }

  public void setEquipedDevice()
  {
    //    if(equipedDevice.GetComponent<EquipmentDevice>
    equipedDevice.transform.parent = equipmentDevice.transform;
    equipedDevice.transform.localPosition = _equipedDeviceDummy.transform.localPosition;
    equipedDevice.transform.localScale = new Vector3(1f, 1f, 0);
    equipedDevice.transform.localRotation = _equipedDeviceDummy.transform.localRotation;
            
    equipedDeviceScript.closeButton = equipmentDeviceScript.closeButton;
    equipmentDeviceScript.closeButton.device = equipedDeviceScript;
    equipedDeviceScript.setDisplayBricks(false);
  }

  //TODO allow multiple protein management
  public void addDisplayedMolecule(DisplayedMolecule molecule)
  {
    _displayedMolecule = molecule;
    molecule.setDisplayType(DisplayedMolecule.DisplayType.DEVICEMOLECULELIST);
  }

  public DisplayedMolecule getDisplayedMolecule()
  {
    return _displayedMolecule;
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