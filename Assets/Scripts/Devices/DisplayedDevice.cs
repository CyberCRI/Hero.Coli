using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayedDevice : DisplayedElement {

  // static stuff

  private static string equipedPrefabURI  = "GUI/screen1/Devices/EquipedDeviceButtonPrefab";
  public static string equipmentPrefabURI = "GUI/screen1/Devices/EquipmentDevicePrefab";

  public static string equipedWithMoleculesPrefabURI = "GUI/screen1/Devices/EquipedDisplayedDeviceWithMoleculesButtonPrefab";
  
  //private static string inventoriedPrefabURI = "GUI/screen1/Devices/InventoriedDeviceButtonPrefab";
  private static string inventoriedPrefabURI = "GUI/screen1/Devices/InventoryDevicePrefab";
  

  private static string listedPrefabURI = "GUI/screen3/Devices/ListedDevicePrefab";

  private static string baseDeviceTextureString = "device_";
  private static string quality256 = "256x256_";
  private static string quality80 = "80x80_";
  private static string quality64 = "64x64_";
  private static string qualityDefault = "64x64_";
  private const float levelLow = .126f;
  private const float levelMed = .23f;
  private const float levelLThreshold = levelLow * 0.75f;
  private const float levelLMThreshold = ( levelLow + levelMed ) / 2;
  private const float levelMThreshold = levelMed * 1.25f;
  private static string levelDefaultPostfix = "";
  private static string levelLowPostfix = "_low";
  private static string levelMedPostfix = "_med";
  private static string defaultTexture = "default";

  private static Dictionary<string, string> geneTextureDico = new Dictionary<string, string>()
  {
    {"FLUO1", "fluo"},
    // TODO fix this to have specific red fluorescence icon
    {"FLUO2", "fluo"},
    {"MOV", "speed"},
    {"AMPR", "resist"}
    //{"REPR1", ?},
    //{"REPR2", ?},
  };


  private static string getTextureName(string proteinName)
  {
    string texture = null;
    if(!geneTextureDico.TryGetValue(proteinName, out texture))
    {
      texture = defaultTexture;
    }
    return texture;
  }

  private static string getLevelPostfix(Device device)
  {
    string postfix;
    if(device == null)
    {
      postfix = levelDefaultPostfix;
    }
    else
    {
      float expressionLevel = device.getExpressionLevel();
  
      if(expressionLevel < levelLThreshold)
      {
        postfix = levelDefaultPostfix;
      }
      else if (expressionLevel < levelLMThreshold)
      {
        postfix = levelLowPostfix;
      }
      else if (expressionLevel < levelMThreshold)
      {
        postfix = levelMedPostfix;
      }
      else
      {
        postfix = levelDefaultPostfix;
      }
    }
    return postfix;
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
    if (deviceType == DevicesDisplayer.DeviceType.Equiped) {
      Debug.LogError("DisplayedDevice: will create Equiped "+equipedPrefabURI);
      prefab = Resources.Load(equipedPrefabURI);
    } else if (deviceType == DevicesDisplayer.DeviceType.Inventoried) {
      Debug.LogError("DisplayedDevice: will create Inventoried "+inventoriedPrefabURI);
      prefab = Resources.Load(inventoriedPrefabURI);
    } else if (deviceType == DevicesDisplayer.DeviceType.Listed) {
      Debug.LogError("DisplayedDevice: will create Listed "+listedPrefabURI);
      prefab = Resources.Load(listedPrefabURI);
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

    DisplayedDevice result = (DisplayedDevice)DisplayedElement.Create(
      parentTransform
      ,localPosition
      ,getTextureName(device)
      ,prefab
      );

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

    if(quality == DevicesDisplayer.TextureQuality.LOW)
      usedSpriteName += getLevelPostfix(device);

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
    Logger.Log("DisplayedDevice::Initialize("+displayedDeviceScript+", "+device+", "+devicesDisplayer+", "+deviceType+") starts", Logger.Level.DEBUG);
    displayedDeviceScript._device = Device.buildDevice(device.getName(), device.getExpressionModules());
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

  }
	
	protected string getDebugInfos() {
		return "device id="+_id+", inner device="+_device+", device type="+_deviceType+", time="+Time.realtimeSinceStartup;
	}
	
	protected override void OnPress(bool isPressed)
  {
    Logger.Log("DisplayedDevice::OnPress "+_id+" device="+_device, Logger.Level.INFO);
  }

  protected virtual void OnHover(bool isOver)
  {
    Logger.Log("DisplayedDevice::OnHover("+isOver+") with _device="+_device, Logger.Level.WARN);
    TooltipManager.displayTooltip(isOver, _device, transform.position);
  }

  //TODO remove tooltip only if tooltip was about this displayed device
  protected virtual void OnDestroy()
  {
    TooltipManager.displayTooltip();
  }
}
