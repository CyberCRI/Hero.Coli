using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class DisplayedDevice : MonoBehaviour {
	
	private static string _activeSuffix = "Active";
	
	public string _currentSpriteName;
	public UIAtlas _atlas;
	public bool _isActive;
	public int _deviceID;
	public UISprite _sprite;
	public Device _device;
	public DevicesDisplayer _devicesDisplayer;
	
	public int getID() {
		return _deviceID;
	}

  public DevicesDisplayer.DeviceType _deviceType;

	public static DisplayedDevice Create(
    DevicesDisplayer.DeviceType deviceType,
		Transform parentTransform, 
		Vector3 localPosition, 
		int deviceID, 
		Device device,
		DevicesDisplayer devicesDisplayer,
		string spriteName
		)
	{

    string nullSpriteName = (spriteName!=null)?"":"(null)";

		Debug.Log("DisplayedDevice::Create(type="+deviceType
		+ ", parentTransform="+parentTransform
		+ ", localPosition="+localPosition
    + ", deviceID="+deviceID
		+ ", device="+device
		+ ", devicesDisplayer="+devicesDisplayer
    + ", spriteName="+spriteName+nullSpriteName);

    Object prefab;

    if (deviceType == DevicesDisplayer.DeviceType.Equiped) {
      prefab = Resources.Load("GUI/screen1/Devices/EquipedDeviceButtonPrefab");
    } else if (deviceType == DevicesDisplayer.DeviceType.Equiped) {
      prefab = Resources.Load("GUI/screen1/Devices/InventoriedDeviceButtonPrefab");
    } else {
      Debug.Log("DisplayedDevice.Create : unmanaged device type "+deviceType);
      return null;
    }

    GameObject newDevice = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
    Debug.Log("instantiation done");

		newDevice.transform.parent = parentTransform;
		newDevice.transform.localPosition = localPosition;
		newDevice.transform.localScale = new Vector3(1f, 1f, 0);
		
	  DisplayedDevice deviceScript = newDevice.GetComponent<DisplayedDevice>();
		deviceScript._deviceID = deviceID;
		deviceScript._device = device;
		deviceScript._devicesDisplayer = devicesDisplayer;
		deviceScript._currentSpriteName = spriteName;
    deviceScript._deviceType = deviceType;

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
    //_currentSpriteName is null
		//Debug.Log("start: _currentSpriteName="+_currentSpriteName+", _sprite.spriteName="+_sprite.spriteName);
		//_sprite.atlas = _atlas;
		setActive();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	protected string getDebugInfos() {
		return "device "+_deviceID+", time="+Time.realtimeSinceStartup;
	}
	
	protected abstract void OnPress(bool isPressed);
}
