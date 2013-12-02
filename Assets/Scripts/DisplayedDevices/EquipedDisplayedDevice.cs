using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipedDisplayedDevice : DisplayedDevice {

  private static string                _activeSuffix = "Active";
  private LinkedList<GenericDisplayedBioBrick> _currentDisplayedBricks = new LinkedList<GenericDisplayedBioBrick>();

  public bool                         _isActive;
  private static GameObject           equipedDevice = null;
  private static GameObject           tinyBioBrickIcon = null;
  private static GameObject           tinyBioBrickIcon2 = null;
  private float                       _tinyIconVerticalShift = 0.0f;
  private static float                _width = 0.0f;

  private static string               _equipedDeviceButtonPrefabPosString = "EquipedDeviceButtonPrefabPos";
  private static string               _tinyBioBrickPosString              = "TinyBioBrickIconPrefabPos";
  private static string               _tinyBioBrickPosString2             = _tinyBioBrickPosString + "2";

  void OnEnable() {
    Logger.Log("EquipedDisplayedDevice::OnEnable "+_device, Logger.Level.TRACE);
    foreach(GenericDisplayedBioBrick brick in _currentDisplayedBricks)
    {
      brick.gameObject.SetActive(true);
    }
  }

  void OnDisable() {
    Logger.Log("EquipedDisplayedDevice::OnDisable "+_device, Logger.Level.TRACE);
    foreach(GenericDisplayedBioBrick brick in _currentDisplayedBricks)
    {
      brick.gameObject.SetActive(false);
    }
  }

  protected override void OnPress(bool isPressed) {
    if(isPressed) {
	    Logger.Log("EquipedDisplayedDevice::OnPress() "+getDebugInfos(), Logger.Level.INFO);
      if(_device == null)
      {
        Logger.Log("EquipedDisplayedDevice::OnPress _device == null", Logger.Level.WARN);
        return;
      }
	    if (_devicesDisplayer.IsEquipScreen()) {
	      _devicesDisplayer.askRemoveEquipedDevice(_device);
	    }
	  }
  }

  public void setActivity(bool activity) {
    _isActive = activity;
    if(activity) {
      setActive();
    } else {
      setInactive();
    }
  }

 public void setActive() {
   Logger.Log("EquipedDisplayedDevice::setActive", Logger.Level.TRACE);
   _isActive = true;
    //TODO FIXME
   //setSprite(_currentSpriteName + _activeSuffix);
 }
 
 public void setInactive() {
   Logger.Log("EquipedDisplayedDevice::setInactive", Logger.Level.TRACE);
   _isActive = false;
    //TODO FIXME
   //setSprite(_currentSpriteName);
 }

  void initIfNecessary() {
    if(equipedDevice == null) {
      equipedDevice = GameObject.Find(_equipedDeviceButtonPrefabPosString);
      tinyBioBrickIcon = GameObject.Find (_tinyBioBrickPosString);
      tinyBioBrickIcon2 = GameObject.Find (_tinyBioBrickPosString2);
    }
    if(_tinyIconVerticalShift == 0.0f)
    {
      _tinyIconVerticalShift = (transform.localPosition - equipedDevice.transform.localPosition).y;
      _width = tinyBioBrickIcon2.transform.localPosition.x - tinyBioBrickIcon.transform.localPosition.x;
    }
  }

  void displayBioBricks() {
    Logger.Log("EquipedDisplayedDevice::displayBioBricks", Logger.Level.DEBUG);
    initIfNecessary();
    if(_device != null)
    {
      //add biobricks
      int index = 0;
      foreach (ExpressionModule module in _device.getExpressionModules())
      {
        foreach(BioBrick brick in module.getBioBricks())
        {
          GenericDisplayedBioBrick dbbrick = TinyBioBrickIcon.Create(transform, getNewPosition(index), null, brick);
          _currentDisplayedBricks.AddLast(dbbrick);
          index++;
        }
      }
    } else {
      Logger.Log("EquipedDisplayedDevice::displayBioBricks _device == null", Logger.Level.WARN);
    }
  }

  //needs tinyBioBrickIcon to be initialized, e.g. using initIfNecessary()
  private Vector3 getNewPosition(int index ) {
    Vector3 shiftPos = new Vector3(index*_width, _tinyIconVerticalShift, -1.0f);
    if(tinyBioBrickIcon == null) {
      Logger.Log("EquipedDisplayedDevice::getNewPosition tinyBioBrickIcon == null", Logger.Level.WARN);
      return new Vector3(index*_width, -95.0f, -0.1f) + shiftPos ;
    } else {
      return tinyBioBrickIcon.transform.localPosition - transform.localPosition + shiftPos;
    }
  }

  // Use this for initialization
  void Start () {
    Logger.Log("EquipedDisplayedDevice::Start", Logger.Level.TRACE);
    setActive();

    initIfNecessary();

    tinyBioBrickIcon.SetActive(false);
    tinyBioBrickIcon2.SetActive(false);
    displayBioBricks();
  }
}