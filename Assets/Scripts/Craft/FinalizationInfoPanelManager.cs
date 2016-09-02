using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinalizationInfoPanelManager : MonoBehaviour {

  //TODO refactor with LastHoveredInfoManager
  private const string            _defaultInfo = "";
  private const string            _defaultName = "no name";
  private const string            _defaultStatus = "-";

  public UISprite finalizationIconSprite;
  public CraftResultDevice craftResultDevice;
  public UILocalize    finalizationStatusLabel;

  private Device    _device;

  public void setDisplayedDevice(
    Device device
    , string status = _defaultStatus
    , string infos = _defaultInfo
    , string name = _defaultName
    ){
    Logger.Log("FinalizationInfoPanelManager::setDisplayedDevice("+device+")"
      +"with status="+status
      +", infos="+infos
      +", name="+name
      +", _device="+_device
      , Logger.Level.DEBUG
      );

    _device = device;
    craftResultDevice._device = device;

    finalizationIconSprite.spriteName = "";//DisplayedDevice.getTextureName(device);
    finalizationIconSprite.gameObject.SetActive(null != device);

    //string displayedInfo = _device!=null?_lengthPrefix+_device.getSize().ToString()+_lengthPostfix:infos;
    //finalizationInfoLabel.text = displayedInfo;
    //Logger.Log("FinalizationInfoPanelManager::setDisplayedDevice: finalizationInfoLabel.text="+finalizationInfoLabel.text, Logger.Level.TRACE);

    //string displayedName = _device!=null?_device.getName():name;
    //finalizationNameLabel.text = displayedName;
    //Logger.Log("FinalizationInfoPanelManager::setDisplayedDevice: finalizationNameLabel.text="+finalizationNameLabel.text, Logger.Level.TRACE);

    if(null != finalizationStatusLabel)
    {
     finalizationStatusLabel.key = status;
     finalizationStatusLabel.Localize();
      //Logger.Log("FinalizationInfoPanelManager::setDisplayedDevice: finalizationStatusLabel.text="+finalizationStatusLabel.text, Logger.Level.TRACE);
    }
  }
}

