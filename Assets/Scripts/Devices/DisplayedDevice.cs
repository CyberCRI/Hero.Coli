using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class DisplayedDevice : DisplayedElement {

	public Device                       _device;
	public DevicesDisplayer             _devicesDisplayer;
  public DevicesDisplayer.DeviceType  _deviceType;

  private static string equipedPrefabURI = "GUI/screen1/Devices/EquipedDeviceButtonPrefab";
  private static string inventoriedPrefabURI = "GUI/screen1/Devices/InventoriedDeviceButtonPrefab";
  private static string listedPrefabURI = "GUI/screen3/Devices/ListedDeviceButtonPrefab";


	public static DisplayedDevice Create(
		Transform parentTransform
		, Vector3 localPosition
    , string spriteName
		, Device device
		, DevicesDisplayer devicesDisplayer
    , DevicesDisplayer.DeviceType deviceType
		)
	{

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
      ,spriteName
      ,prefab
      );

    Initialize(result, device, devicesDisplayer, deviceType);

    return result;
	}

  public static void Initialize(
    DisplayedDevice displayedDeviceScript
    , Device device
    , DevicesDisplayer devicesDisplayer
    , DevicesDisplayer.DeviceType deviceType
    ) {

    Logger.Log("DisplayedDevice::Initialize("+displayedDeviceScript+", "+device+", "+devicesDisplayer+", "+deviceType+") starts", Logger.Level.TRACE);
    displayedDeviceScript._device = Device.buildDevice(device.getName(), device.getExpressionModules());
    Logger.Log("DisplayedDevice::Create built device "+displayedDeviceScript._device+" from "+device, Logger.Level.TRACE);
    displayedDeviceScript._devicesDisplayer = devicesDisplayer;
    displayedDeviceScript._deviceType = deviceType;
    Logger.Log("DisplayedDevice::Initialize ends", Logger.Level.TRACE);

  }
	
	protected string getDebugInfos() {
		return "device id="+_id+", inner device="+_device+", device type="+_deviceType+", time="+Time.realtimeSinceStartup;
	}
	
	protected override abstract void OnPress(bool isPressed);
}
