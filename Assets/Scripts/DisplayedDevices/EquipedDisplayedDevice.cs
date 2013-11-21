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
   setSprite(_currentSpriteName + _activeSuffix);
 }
 
 public void setInactive() {
   Logger.Log("EquipedDisplayedDevice::setInactive", Logger.Level.TRACE);
   _isActive = false;
   setSprite(_currentSpriteName);
 }

  // Use this for initialization
  void Start () {
    Logger.Log("EquipedDisplayedDevice::Start", Logger.Level.TRACE);
    setActive();

    if(equipedDevice == null) {
      equipedDevice = GameObject.Find("EquipedDeviceButtonPrefabPos");
      tinyBioBrickIcon = GameObject.Find ("TinyBioBrickIconPrefabPos");
      tinyBioBrickIcon2 = GameObject.Find ("TinyBioBrickIconPrefabPos2");
    }
    if(_tinyIconVerticalShift == 0.0f)
    {
      _tinyIconVerticalShift = (transform.localPosition - equipedDevice.transform.localPosition).y;
      _width = tinyBioBrickIcon2.transform.localPosition.x - tinyBioBrickIcon.transform.localPosition.x;
    }
    tinyBioBrickIcon.SetActive(false);
    tinyBioBrickIcon2.SetActive(false);
    displayBioBricks();
  }

  void displayBioBricks() {
    Logger.Log("EquipedDisplayedDevice::displayBioBricks", Logger.Level.DEBUG);
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

  private Vector3 getNewPosition(int index ) {
    //Vector3 defaultPos = new Vector3(index*_width, -95.0f, -0.1f);
    Vector3 shiftPos = new Vector3(index*_width, _tinyIconVerticalShift, -0.1f);
    if(tinyBioBrickIcon == null) {
      Logger.Log("EquipedDisplayedDevice::getNewPosition tinyBioBrickIcon == null", Logger.Level.WARN);
      return new Vector3(index*_width, -95.0f, -0.1f) + shiftPos ;
    } else {
      return tinyBioBrickIcon.transform.localPosition - transform.localPosition + shiftPos;
    }
  }

}