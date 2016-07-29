using UnityEngine;
using System.Collections.Generic;

public class DisplayedDevice : DisplayedElement {

    public UISprite levelSprite;
    public UISprite backgroundSprite;
    public UILocalize moleculeOverlay;

  // prefab URIs
  private const string equipedPrefabURI  = "GUI/screen1/Devices/DisplayedDevicePrefab";
  public const string equipmentPrefabURI = "GUI/screen1/Devices/EquipmentDevicePrefab";
  public const string equipedWithMoleculesPrefabURI = "GUI/screen1/Devices/EquipedDisplayedDeviceWithMoleculesPrefab";  
  //private const string inventoriedPrefabURI = "GUI/screen1/Devices/InventoriedDeviceButtonPrefab";
  private const string inventoriedPrefabURI = "GUI/screen1/Devices/InventoryDevicePrefab";
  private const string listedPrefabURI = "GUI/screen3/Devices/ListedDevicePrefab";

  // device icon //
  
  // common
  private const string baseDeviceTextureString = "device_";
  private const string quality256 = "256x256_";
  private const string quality80 = "80x80_";
  private const string quality64 = "64x64_";
  private const string qualityDefault = quality64;
  
  // level
  private const float levelBase = .06f;
  private const float levelLow = .126f;
  private const float levelMed = .23f;
  private const float levelBThreshold = levelBase * 0.75f;
  private const float levelBLThreshold = ( levelBase + levelLow ) / 2;
  private const float levelLMThreshold = ( levelLow + levelMed ) / 2;
  private const float levelMThreshold = levelMed * 1.25f;
  private const string levelBaseSuffix = "_base";
  private const string levelLowSuffix = "_low";
  private const string levelMedSuffix = "_med";
  private const string level1Suffix = "level1";
  private const string level2Suffix = "level2";
  private const string level3Suffix = "level3";
  private const string textSuffix = "_text";
  private const string pictureSuffix = "_picture";
  private const string level1TextSpriteName = baseDeviceTextureString+qualityDefault+level1Suffix+textSuffix;
  private const string level2TextSpriteName = baseDeviceTextureString+qualityDefault+level2Suffix+textSuffix;
  private const string level3TextSpriteName = baseDeviceTextureString+qualityDefault+level3Suffix+textSuffix;
  private const string level1PictureSpriteName = baseDeviceTextureString+qualityDefault+level1Suffix+pictureSuffix;
  private const string level2PictureSpriteName = baseDeviceTextureString+qualityDefault+level2Suffix+pictureSuffix;
  private const string level3PictureSpriteName = baseDeviceTextureString+qualityDefault+level3Suffix+pictureSuffix;
  
  // default texture
  private const string defaultTexture = "default";
  private const string defaultTextureWithText = defaultTexture+textSuffix;
  
  // molecule name overlay
  private const string moleculeOverlayPrefix = "BRICK.ICONLABEL.";
  
  // background
  private const string backgroundSuffix = "_background";
  private const string squareBackgroundSuffix = "_square";
  private const string roundedSquareBackgroundSuffix = "_rounded_square";
  private const string circleBackgroundSuffix = "_circle";
  private const string squareBackgroundSpriteName = baseDeviceTextureString+qualityDefault+backgroundSuffix+squareBackgroundSuffix;
  private const string roundedSquareBackgroundSpriteName = baseDeviceTextureString+qualityDefault+backgroundSuffix+roundedSquareBackgroundSuffix;
  private const string circleBackgroundSpriteName = baseDeviceTextureString+qualityDefault+backgroundSuffix+circleBackgroundSuffix;
  
  private static Dictionary<string, string> geneSpecialTextureDico = new Dictionary<string, string>()
  {
        {"ARAC", defaultTextureWithText},
        {"ATC", defaultTextureWithText},
        {"IPTG", defaultTextureWithText},
        {"LARA", defaultTextureWithText},
        {"REPR1", defaultTextureWithText},
        {"REPR2", defaultTextureWithText},
        {"REPR3", defaultTextureWithText},
        {"REPR4", defaultTextureWithText}
  };

  private static string getTextureName(string proteinName)
  {
    string texture = defaultTextureWithText;
    if(!geneSpecialTextureDico.TryGetValue(proteinName, out texture))
    {
      texture = proteinName.ToLowerInvariant();
    }
    return texture;
  }
  
  private void setBackgroundSprite(string backgroundSpriteName = circleBackgroundSpriteName)
  {
      if(null != backgroundSprite)
      {
          levelSprite.spriteName = backgroundSpriteName;
          levelSprite.gameObject.SetActive(true);
      }
  }

    private void setMoleculeOverlay(string proteinName)
    {
        if (null != moleculeOverlay)
        {
            string texture = defaultTextureWithText;
            if (geneSpecialTextureDico.TryGetValue(proteinName, out texture))
            {
                moleculeOverlay.gameObject.SetActive(true);
                moleculeOverlay.key = moleculeOverlayPrefix + proteinName.ToUpperInvariant();
            }
            else
            {
                moleculeOverlay.gameObject.SetActive(false);
            }
        }
    }
  
  private void setLevelSprite(bool isPictureMode = true)
  {
      if(null != levelSprite)
      {
          string spriteName = getLevelSpriteName(_device, isPictureMode);
          if(!string.IsNullOrEmpty(spriteName))
          {
              levelSprite.spriteName = spriteName;
              levelSprite.gameObject.SetActive(true);
          }
          else
          {
              levelSprite.gameObject.SetActive(false);
          }
      }
  }
  
  private void setDeviceIcon()
  {   
        _currentSpriteName = getTextureName(_device);
        setSprite(_currentSpriteName);
  }

  private static string getLevelSpriteName(Device device, bool isPictureMode = true)
  {
    string spriteName;
    if(device == null)
    {
      spriteName = "";
    }
    else
    {
      float expressionLevel = device.getExpressionLevel();
  
      if(expressionLevel < levelBThreshold)
      {
        spriteName = "";
      }
      else if (expressionLevel < levelBLThreshold)
      {
        spriteName = isPictureMode?level1PictureSpriteName:level1TextSpriteName;
      }
      else if (expressionLevel < levelLMThreshold)
      {
        spriteName = isPictureMode?level2PictureSpriteName:level2TextSpriteName;
      }
      else if (expressionLevel < levelMThreshold)
      {
        spriteName = isPictureMode?level3PictureSpriteName:level3TextSpriteName;
      }
      else
      {
        spriteName = "";
      }
    }
    return spriteName;
  }

  public Device                       _device;
  protected static DevicesDisplayer   _devicesDisplayer;
  public DevicesDisplayer.DeviceType  _deviceType;

	public static DisplayedDevice Create(
            Transform parentTransform
            , Vector3 localPosition
            , string spriteName
            , Device device
            , DevicesDisplayer devicesDisplayer
            , DevicesDisplayer.DeviceType deviceType
        )
	{

    if(device == null)
    {
      Logger.Log ("DisplayedDevice::Create device==null", Logger.Level.WARN);
    }

    string nullSpriteName = (spriteName!=null)?"":"(null)";
    Object prefab;
    string name = "";
    if (deviceType == DevicesDisplayer.DeviceType.Equiped) {
            Logger.Log("DisplayedDevice: will create Equiped "+equipedPrefabURI, Logger.Level.DEBUG);
      prefab = Resources.Load(equipedPrefabURI);
      name = "Equiped";
    } else if (deviceType == DevicesDisplayer.DeviceType.Inventoried) {
            Logger.Log("DisplayedDevice: will create Inventoried "+inventoriedPrefabURI, Logger.Level.DEBUG);
      prefab = Resources.Load(inventoriedPrefabURI);
      name = "Inventoried";
    } else if (deviceType == DevicesDisplayer.DeviceType.Listed) {
            Logger.Log("DisplayedDevice: will create Listed "+listedPrefabURI, Logger.Level.DEBUG);
      prefab = Resources.Load(listedPrefabURI);
      name = "Listed";
    } else {
      Logger.Log("DisplayedDevice::Create : unmanaged device type "+deviceType, Logger.Level.WARN);
      return null;
    }

		Logger.Log("DisplayedDevice::Create(type="+deviceType
		+ ", parentTransform="+parentTransform
		+ ", localPosition="+localPosition
		+ ", device="+device
		+ ", devicesDisplayer="+devicesDisplayer
    + ", spriteName="+spriteName+nullSpriteName
    , Logger.Level.DEBUG
    );
    
    name += device.getInternalName();

    DisplayedDevice result = (DisplayedDevice)DisplayedElement.Create(
      parentTransform
      ,localPosition
      ,getTextureName(device)
      ,prefab
      );

      result.name = name;

    Initialize(result, device, devicesDisplayer, deviceType);

    return result;
	}

  public static string getTextureName(Device device)
  {
    string usedSpriteName = baseDeviceTextureString;

    DevicesDisplayer.TextureQuality quality = DevicesDisplayer.getTextureQuality();

    switch (quality)
    {
      case DevicesDisplayer.TextureQuality.HIGH:
        usedSpriteName += quality256;
        break;
      case DevicesDisplayer.TextureQuality.NORMAL:
        usedSpriteName += quality80;
        break;
      case DevicesDisplayer.TextureQuality.LOW:
        usedSpriteName += quality64;
        break;
      default:
        usedSpriteName += qualityDefault;
        break;
    }

    if(null == device)
    {
      usedSpriteName += getTextureName("");
    }
    else
    {
      usedSpriteName += getTextureName(device.getFirstGeneProteinName());
    }

/*
    if(quality == DevicesDisplayer.TextureQuality.LOW)
      usedSpriteName += getLevelSuffix(device);
*/

    Logger.Log("DisplayedDevice::getTextureName usedSpriteName="+usedSpriteName, Logger.Level.TRACE);
    return usedSpriteName;
  }

  public static void Initialize(
    DisplayedDevice displayedDeviceScript
    , Device device
    , DevicesDisplayer devicesDisplayer
    , DevicesDisplayer.DeviceType deviceType
    ) {

    if(device == null)
    {
      Logger.Log("DisplayedDevice::Initialize device==null", Logger.Level.WARN);
    }
    if(null != devicesDisplayer)
        Logger.Log("DisplayedDevice::Initialize("+displayedDeviceScript+", "+device+", "+devicesDisplayer+", "+deviceType+") starts", Logger.Level.DEBUG);
        
    displayedDeviceScript._device = Device.buildDevice(device);
    if(displayedDeviceScript._device==null)
    {
      Logger.Log("DisplayedDevice::Initialize _device==null", Logger.Level.WARN);
    }
    Logger.Log("DisplayedDevice::Create built device "+displayedDeviceScript._device+" from "+device, Logger.Level.TRACE);
    if(_devicesDisplayer == null)
    {
      _devicesDisplayer = DevicesDisplayer.get();
    }
    displayedDeviceScript._deviceType = deviceType;
    Logger.Log("DisplayedDevice::Initialize ends", Logger.Level.TRACE);
    
    displayedDeviceScript.setBackgroundSprite();
    displayedDeviceScript.setMoleculeOverlay(device.getFirstGeneProteinName());
    displayedDeviceScript.setLevelSprite();
    displayedDeviceScript.setDeviceIcon();
  }
	
	protected string getDebugInfos() {
		return "device id="+_id+", inner device="+_device+", device type="+_deviceType+", time="+Time.realtimeSinceStartup;
	}
    
    public void toggleEquiped()
    {
        if (_device == null)
        {
            Logger.Log("DisplayedDevice::toggleEquiped _device==null", Logger.Level.WARN);
            return;
        }

        DeviceContainer.AddingResult addingResult = _devicesDisplayer.askAddEquipedDevice(_device);
        Logger.Log("DisplayedDevice::toggleEquiped added device result=" + addingResult + ", " + getDebugInfos(), Logger.Level.DEBUG);
        if (DeviceContainer.AddingResult.FAILURE_SAME_NAME == addingResult
           || DeviceContainer.AddingResult.FAILURE_SAME_DEVICE == addingResult)
        {
            if (_devicesDisplayer.askRemoveEquipedDevice(_device))
            {
                RedMetricsManager.get().sendEvent(TrackingEvent.UNEQUIP, new CustomData(CustomDataTag.DEVICE, _device.getInternalName()));
            }
        }
        else
        {
            RedMetricsManager.get().sendEvent(TrackingEvent.EQUIP, new CustomData(CustomDataTag.DEVICE, _device.getInternalName()));
        }
    }
	
	public override void OnPress(bool isPressed)
  {
    Logger.Log("DisplayedDevice::OnPress "+_id+" device="+_device, Logger.Level.INFO);
  }

  protected virtual void OnHover(bool isOver)
  {
    Logger.Log("DisplayedDevice::OnHover("+isOver+") with _device="+_device, Logger.Level.INFO);
    TooltipManager.displayTooltip(isOver, _device, transform.position);
  }

  //TODO remove tooltip only if tooltip was about this displayed device
  protected virtual void OnDestroy()
  {
    TooltipManager.displayTooltip();
  }
}
