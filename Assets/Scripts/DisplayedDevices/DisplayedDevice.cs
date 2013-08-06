using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class DisplayedDevice : MonoBehaviour {
	
	private static string _activeSuffix = "Active";
  private static int _idCounter = 0;
	
	public string _currentSpriteName;
	public UIAtlas _atlas;
	public bool _isActive;
	public int _deviceID;
	public UISprite _sprite;
	public Device _device;
	public DevicesDisplayer _devicesDisplayer;

  public DevicesDisplayer.DeviceType _deviceType;
 
  public int getID() {
    return _deviceID;
  }

	public static DisplayedDevice Create(
    DevicesDisplayer.DeviceType deviceType,
		Transform parentTransform, 
		Vector3 localPosition,
		Device device,
		DevicesDisplayer devicesDisplayer,
		string spriteName
		)
	{

    string nullSpriteName = (spriteName!=null)?"":"(null)";

		Debug.Log("DisplayedDevice::Create(type="+deviceType
		+ ", parentTransform="+parentTransform
		+ ", localPosition="+localPosition
		+ ", device="+device
		+ ", devicesDisplayer="+devicesDisplayer
    + ", spriteName="+spriteName+nullSpriteName);

    Object prefab;

    if (deviceType == DevicesDisplayer.DeviceType.Equiped) {
      prefab = Resources.Load("GUI/screen1/Devices/EquipedDeviceButtonPrefab");
    } else if (deviceType == DevicesDisplayer.DeviceType.Inventoried) {
      prefab = Resources.Load("GUI/screen1/Devices/InventoriedDeviceButtonPrefab");
    } else {
      Debug.Log("DisplayedDevice::Create : unmanaged device type "+deviceType);
      return null;
    }

    GameObject newDevice = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
    Debug.Log("DisplayedDevice::Create instantiation done");

		newDevice.transform.parent = parentTransform;
		newDevice.transform.localPosition = localPosition;
		newDevice.transform.localScale = new Vector3(1f, 1f, 0);
		
	  DisplayedDevice deviceScript = newDevice.GetComponent<DisplayedDevice>();
		deviceScript._deviceID = ++_idCounter;
    Debug.Log("DisplayedDevice::Create deviceScript._deviceID = "+deviceScript._deviceID);

		deviceScript._device = Device.buildDevice(device.getName(), device.getExpressionModules());
    Debug.Log("DisplayedDevice::Create built device "+deviceScript._device+" from "+device);
		deviceScript._devicesDisplayer = devicesDisplayer;
		deviceScript._currentSpriteName = spriteName;
    deviceScript._deviceType = deviceType;
    deviceScript.setActive();
    Debug.Log("DisplayedDevice::Create ends");

    return deviceScript;
	}
	
	public void Remove() {
		Destroy(gameObject);
	}
	
	public void Redraw(Vector3 newLocalPosition) {
		gameObject.transform.localPosition = newLocalPosition;
	}
	
	private void setSprite(string spriteName) {
		Debug.Log("setSprite("+spriteName+")");
		_sprite.spriteName = spriteName;
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
		Debug.Log("setActive");
		_isActive = true;
		setSprite(_currentSpriteName + _activeSuffix);		
	}
	
	public void setInactive() {
		Debug.Log("setInactive");
		_isActive = false;
		setSprite(_currentSpriteName);
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	protected string getDebugInfos() {
		return "device "+_deviceID+", inner device "+_device+" time="+Time.realtimeSinceStartup;
	}
	
	protected abstract void OnPress(bool isPressed);
}
