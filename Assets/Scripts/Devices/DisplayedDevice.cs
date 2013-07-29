using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayedDevice : MonoBehaviour {
	
	private static string _activeSuffix = "Active";
	
	public string _currentSpriteName;
	public UIAtlas _atlas;
	public bool _isActive;
	public int _deviceID;
	public UISprite _sprite;
	public DevicesDisplayer.DeviceType _deviceType;
	public DeviceInfo _deviceInfo;
	public DevicesDisplayer _devicesDisplayer;
	
	
	public int getID() {
		return _deviceID;
	}
	
	public static Object prefab = Resources.Load("GUI/screen1/EquipedDevices/DeviceButtonPrefab");
	public static DisplayedDevice Create(
		Transform parentTransform, 
		Vector3 localPosition, 
		int deviceID, 
		DevicesDisplayer.DeviceType deviceType,
		DeviceInfo deviceInfo,
		DevicesDisplayer devicesDisplayer
		)
	{
		Debug.Log("create device "+deviceID
		+ " parentTransform="+parentTransform
		+ " localPosition="+localPosition 
		+ " deviceType="+deviceType
		+ "deviceInfo="+deviceInfo
		+ "devicesDisplayer="+devicesDisplayer);
		
	    GameObject newDevice = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		newDevice.transform.parent = parentTransform;
		newDevice.transform.localPosition = localPosition;
		newDevice.transform.localScale = new Vector3(1f, 1f, 0);
		
	    DisplayedDevice deviceScript = newDevice.GetComponent<DisplayedDevice>();
		deviceScript._deviceID = deviceID;
		deviceScript._deviceType = deviceType;
		deviceScript._deviceInfo = deviceInfo;
		deviceScript._devicesDisplayer = devicesDisplayer;
		deviceScript._currentSpriteName = deviceInfo._spriteName;
		/*
		//deviceScript._sprite = newDevice.transform.GetComponentInChildren<UISprite>();
		GameObject background = newDevice.transform.Find("Background").gameObject;
		if(background) {
			deviceScript._sprite = background.GetComponent<UISprite>();
			Debug.Log("init _sprite as "+deviceScript._sprite);
		} else {
			Debug.Log("init _sprite impossible: no background found");
		}
		*/
	 
	    //do additional initialization steps here
	 
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
		Debug.Log("start: _currentSpriteName="+_currentSpriteName+", _sprite.spriteName="+_sprite.spriteName);
		_sprite.atlas = _atlas;
		setActive();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	string getDebugInfos() {
		return "device "+_deviceID+", time="+Time.realtimeSinceStartup;
	}
	
	void OnPress(bool isPressed) {
		if(isPressed) {
			Debug.Log("on press() "+getDebugInfos());
			if(_deviceType == DevicesDisplayer.DeviceType.Inventory) {
				_devicesDisplayer.addDevice(0, _deviceInfo, DevicesDisplayer.DeviceType.Equiped);
			} else if (_deviceType == DevicesDisplayer.DeviceType.Equiped) {
				if (_devicesDisplayer.IsEquipScreen()) _devicesDisplayer.removeDevice(_deviceID);
			} else {
				Debug.Log("device type "+_deviceType+" not managed in OnPress");
			}
		}
	}
}
