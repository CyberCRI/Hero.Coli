using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipedDisplayedDevice : DisplayedDevice {
  private LinkedList<GenericDisplayedBioBrick> _currentDisplayedBricks = new LinkedList<GenericDisplayedBioBrick>();
    	
  private static GameObject           equipedDevice;
  private static GameObject           tinyBioBrickIcon;
  private static GameObject           tinyBioBrickIcon2;
  private static float                _tinyIconVerticalShift = 0.0f;
  private static float                _width = 0.0f;

  private static string               _equipedDeviceButtonPrefabPosString = "EquipedDeviceButtonPrefabPos";
  private static string               _tinyBioBrickPosString              = "TinyBioBrickIconPrefabPos";
  private static string               _tinyBioBrickPosString2             = _tinyBioBrickPosString + "2";

  private bool                        _displayBricks;
  private bool                        _isEquipScreen;


  public EquipedDeviceCloseButton closeButton;

  void OnEnable() {
    Logger.Log("EquipedDisplayedDevice::OnEnable "+_device, Logger.Level.TRACE);
    createBioBricksIfNecessary();
    updateVisibility();
  }

  void OnDisable() {
    Logger.Log("EquipedDisplayedDevice::OnDisable "+_device, Logger.Level.TRACE);
    setBricksVisibilityTo(false);
  }

  protected void setBricksVisibilityTo(bool visible)
  {
    foreach(GenericDisplayedBioBrick brick in _currentDisplayedBricks)
    {
      brick.gameObject.SetActive(visible);
    }
  }

  protected void updateVisibility()
  {
    setBricksVisibilityTo(_displayBricks);
  }

  public override void OnPress(bool isPressed) {
    if(isPressed) {
	    Logger.Log("EquipedDisplayedDevice::OnPress() "+getDebugInfos(), Logger.Level.INFO);
      if(_device == null)
      {
        Logger.Log("EquipedDisplayedDevice::OnPress _device == null", Logger.Level.WARN);
        return;
      }      
            if(_devicesDisplayer.IsEquipScreen()) {
                if(askRemoveDevice())
                {
                    RedMetricsManager.get ().sendEvent(TrackingEvent.UNEQUIP, new CustomData(CustomDataTag.DEVICE, _device.getInternalName()));
                }
            }
      }
  }

  public bool askRemoveDevice()
  {        
        TooltipManager.displayTooltip();
        return _devicesDisplayer.askRemoveEquipedDevice(_device);
  }

  void initIfNecessary() {
    Logger.Log("EquipedDisplayedDevice::initIfNecessary starts", Logger.Level.INFO);
    if(
        (null == equipedDevice)
        || (null == tinyBioBrickIcon)
        || (null == tinyBioBrickIcon2)
        || (0 == _tinyIconVerticalShift)
        || (0 == _width)
      ) 
    {
			equipedDevice = DevicesDisplayer.get().equipedDevice;

			tinyBioBrickIcon = GameObject.Find("InterfaceLinkManager").GetComponent<InterfaceLinkManager>().tinyBioBrickIconPrefabPos;
			tinyBioBrickIcon2 = GameObject.Find("InterfaceLinkManager").GetComponent<InterfaceLinkManager>().tinyBioBrickIconPrefabPos2;
    }
    
    if(null != tinyBioBrickIcon)
    {
      if(null != equipedDevice)
      {
        _tinyIconVerticalShift = (tinyBioBrickIcon.transform.localPosition - equipedDevice.transform.localPosition).y;
      }
      if (null != tinyBioBrickIcon2)
      {  
        _width = tinyBioBrickIcon2.transform.localPosition.x - tinyBioBrickIcon.transform.localPosition.x;
        tinyBioBrickIcon.SetActive(false);
        tinyBioBrickIcon2.SetActive(false);
      }
      Logger.Log("EquipedDisplayedDevice::initIfNecessary ends", Logger.Level.INFO);
    }
  }

  void createBioBricksIfNecessary() {
    Logger.Log("EquipedDisplayedDevice::createBioBricksIfNecessary", Logger.Level.DEBUG);
    initIfNecessary();
    if(0 == _currentDisplayedBricks.Count)
    {
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
        Logger.Log("EquipedDisplayedDevice::createBioBricksIfNecessary _device == null", Logger.Level.WARN);
      }
    }
  }

  //needs tinyBioBrickIcon to be initialized, e.g. using initIfNecessary()
  private Vector3 getNewPosition(int index ) {
    Vector3 shiftPos = new Vector3(index*_width, _tinyIconVerticalShift, -1.0f);
    if(tinyBioBrickIcon == null) {
      Logger.Log("EquipedDisplayedDevice::getNewPosition tinyBioBrickIcon == null", Logger.Level.WARN);
      return new Vector3(index*_width, -95.0f, -0.1f) + shiftPos ;
    } else {
      //return tinyBioBrickIcon.transform.localPosition + shiftPos;
			return shiftPos;
    }
  }

  public void setDisplayBricks(bool display)
  {
    _displayBricks = display;
    updateVisibility();
  }
    
  protected override void OnHover(bool isOver)
  {
    Logger.Log("EquipedDisplayedDevice::OnHover("+isOver+") with _device="+_device, Logger.Level.INFO);
    base.OnHover(isOver);
    
    if(null != closeButton && !_devicesDisplayer.IsEquipScreen())
    {
      //TODO fix interaction with Update
      closeButton.gameObject.SetActive(isOver);
    }
  }

  void Update () {
    bool newIsEquipScreen = _devicesDisplayer.IsEquipScreen();
    if(null != closeButton)
    {
      if(_isEquipScreen && _isEquipScreen != newIsEquipScreen)
      {
        closeButton.gameObject.SetActive(false);
      }
      else
      {
        if(!_isEquipScreen && _isEquipScreen != newIsEquipScreen)
        {
          //TODO fix interaction with OnHover
          closeButton.gameObject.SetActive(true);
        }
      }
    }
    _isEquipScreen = newIsEquipScreen;

    //no-hover version
    //closeButton.gameObject.SetActive(_devicesDisplayer.IsEquipScreen());    
  }

  // Use this for initialization
  void Start () {
    Logger.Log("EquipedDisplayedDevice::Start", Logger.Level.TRACE);
    createBioBricksIfNecessary();
    updateVisibility();
  }
}