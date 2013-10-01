using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class DisplayedDevice : DisplayedElement {

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

    string nullSpriteName = (spriteName!=null)?"":"(null)";
    Object prefab;
    if (deviceType == DevicesDisplayer.DeviceType.Equiped) {
      prefab = Resources.Load("GUI/screen1/Devices/EquipedDeviceButtonPrefab");
    } else if (deviceType == DevicesDisplayer.DeviceType.Inventoried) {
      prefab = Resources.Load("GUI/screen1/Devices/InventoriedDeviceButtonPrefab");
    } else if (deviceType == DevicesDisplayer.DeviceType.Listed) {
      prefab = Resources.Load("GUI/screen3/Devices/ListedDeviceButtonPrefab");
    } else {
      Debug.Log("DisplayedDevice::Create : unmanaged device type "+deviceType);
      return null;
    }

		Debug.Log("DisplayedDevice::Create(type="+deviceType
		+ ", parentTransform="+parentTransform
		+ ", localPosition="+localPosition
		+ ", device="+device
		+ ", devicesDisplayer="+devicesDisplayer
    + ", spriteName="+spriteName+nullSpriteName);

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

    Debug.Log("DisplayedDevice::Initialize("+displayedDeviceScript+", "+device+", "+devicesDisplayer+", "+deviceType+") starts");
    displayedDeviceScript._device = Device.buildDevice(device.getName(), device.getExpressionModules());
    Debug.Log("DisplayedDevice::Create built device "+displayedDeviceScript._device+" from "+device);
    displayedDeviceScript._devicesDisplayer = devicesDisplayer;
    displayedDeviceScript._deviceType = deviceType;
    Debug.Log("DisplayedDevice::Initialize ends");

  }
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	protected string getDebugInfos() {
		return "device id="+_id+", inner device="+_device+", device type="+_deviceType+", time="+Time.realtimeSinceStartup;
	}
	
	protected override abstract void OnPress(bool isPressed);
}
