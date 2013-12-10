using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayedDevice : DisplayedElement {

  // static stuff
  private static string equipedPrefabURI = "GUI/screen1/Devices/EquipedDeviceButtonPrefab";
  private static string inventoriedPrefabURI = "GUI/screen1/Devices/InventoriedDeviceButtonPrefab";
  private static string listedPrefabURI = "GUI/screen3/Devices/ListedDeviceButtonPrefab";

  private static string baseDeviceTextureString = "device_";
  private static string quality256 = "256x256_";
  private static string quality80 = "80x80_";
  private static string defaultTexture = "default";

  private static Dictionary<string, string> geneTextureDico = new Dictionary<string, string>()
  {
    {"MOV", "speed"},
    //{"Y", "default"},
    {"Z", "resist"},
    {"GFP", "fluo"}
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

  public Device                       _device;
  public DevicesDisplayer             _devicesDisplayer;
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
      prefab = Resources.Load(equipedPrefabURI);
    } else if (deviceType == DevicesDisplayer.DeviceType.Inventoried) {
      prefab = Resources.Load(inventoriedPrefabURI);
    } else if (deviceType == DevicesDisplayer.DeviceType.Listed) {
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
    switch (DevicesDisplayer.getTextureQuality())
    {
      case DevicesDisplayer.TextureQuality.HIGH:
        usedSpriteName += quality256;
        break;
      case DevicesDisplayer.TextureQuality.NORMAL:
        usedSpriteName += quality80;
        break;
      default:
        usedSpriteName += quality80;
        break;
    }
    usedSpriteName += getTextureName(device.getFirstGeneProteinName());
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
    displayedDeviceScript._devicesDisplayer = devicesDisplayer;
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

  void OnHover(bool isOver)
  {
    Logger.Log("DisplayedDevice::OnHover("+isOver+")", Logger.Level.DEBUG);
    TooltipManager.tooltip(isOver, _device, transform.position);
  }
}
