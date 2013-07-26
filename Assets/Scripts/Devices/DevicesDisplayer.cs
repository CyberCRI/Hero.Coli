using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DevicesDisplayer : MonoBehaviour {
	
	public enum DeviceType {
		Equiped,
		Inventory
	}
	
	private List<Device> _equipedDevices = new List<Device>();
	private List<Device> _inventoryDevices = new List<Device>();
	
	private float _timeAtLastFrame = 0f;
    private float _timeAtCurrentFrame = 0f;
    private float _deltaTime = 0f;	
	private float _deltaTimeThreshold = 0.2f;
	
	//TODO use real device width
	static private float _height = 52.0f;
	public ReactionEngine reactionEngine;
	public int celliaMediumID = 1;
	public UIPanel equipPanel;
	public GameObject equipment;
	public GameObject inventory;
	public UIPanel inventoryPanel;
	public GameObject equipedDevice;	
	public GameObject inventoryDevice;
	
	
	public void addDevice(int deviceID, DeviceInfo deviceInfo, DeviceType deviceType) {
		
		Debug.Log("addDevice("+deviceID+", "+deviceInfo+", "+deviceType+")");
		bool alreadyEquiped = (!_equipedDevices.Exists(device => device.getID() == deviceID));
		bool alreadyInventory = (!_inventoryDevices.Exists(device => device.getID() == deviceID)); 
		if(alreadyEquiped || alreadyInventory) { 
			Vector3 localPosition;
			UnityEngine.Transform parent;
			List<Device> devices;
			int newDeviceId = deviceID;
			if(deviceType == DeviceType.Equiped) {
				parent = equipPanel.transform;
				devices = _equipedDevices;
				if(deviceID == 0) {
					newDeviceId = devices.Count;
				}
				Debug.Log("addDevice("+newDeviceId+") in equipment");
			} else {			
				parent = inventoryPanel.transform;
				devices = _inventoryDevices;
				Debug.Log("addDevice("+newDeviceId+") in inventory");
			}
			localPosition = getNewPosition(deviceType);
			Device newDevice = Device.Create (parent, localPosition, newDeviceId, deviceType, deviceInfo, this);
			devices.Add(newDevice);
			//let's add reaction to reaction engine
			//for each module of deviceInfo, add to reaction engine
			//deviceInfo._modules.ForEach( module => module.addToReactionEngine(celliaMediumID, reactionEngine));
		} else {
			Debug.Log("addDevice failed: alreadyEquiped="+alreadyEquiped+", alreadyInventory="+alreadyInventory);
		}
	}
	
	public void UpdateScreen(int screenID){
		Debug.Log(screenID + " screen");
		if(screenID == 1 || screenID == 3){
			//inventoryPanel.SetAlphaRecursive(0,true);
			inventoryPanel.gameObject.SetActive(false);
		}
		if(screenID == 2){
			//inventoryPanel.SetAlphaRecursive(100,true);
			inventoryPanel.gameObject.SetActive(true);
		}
	}
	
	public Vector3 getNewPosition(DeviceType deviceType) {
		Vector3 res;
		if(deviceType == DeviceType.Equiped) {
			res = equipedDevice.transform.localPosition + new Vector3(0.0f, -_equipedDevices.Count*_height, -0.1f);
		} else {
			res = inventoryDevice.transform.localPosition + new Vector3((_inventoryDevices.Count%3)*_height, -(_inventoryDevices.Count/3)*_height, -0.1f);
		}		
		return res;
	}
	
	public void removeDevice(int deviceID) {
		Debug.Log("removeDevice("+deviceID+")");
		Device toRemove = _equipedDevices.Find(device => device.getID() == deviceID);
		List<Device> devices = _equipedDevices;
		Vector3 position = equipedDevice.transform.localPosition;
		DeviceType deviceType = DeviceType.Equiped;
		if(toRemove == null) {
			toRemove = _inventoryDevices.Find(device => device.getID() == deviceID);
			devices = _inventoryDevices;
			position = inventoryDevice.transform.localPosition;
			deviceType = DeviceType.Inventory;
		}
		
		if(toRemove != null) {
			devices.Remove(toRemove);
			toRemove.Remove();
			for(int i = 0; i < devices.Count; i++) {
				Vector3 newLocalPosition = getNewPosition(deviceType);
				devices[i].Redraw(newLocalPosition);
			}
		}
	}
	
	// Use this for initialization
	void Start () {		
		//inventoryPanel.SetAlphaRecursive(0,true);//screen 1 at the beginning = no inventory
		inventoryPanel.gameObject.SetActive(false);
		/*
		for(int i = 0; i < 5; i++) {
			addDevice (i);
		}
		*/
	}

	
	// Update is called once per frame
	void Update () {
		
		_timeAtCurrentFrame = Time.realtimeSinceStartup;
        _deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;		
		
		if(_deltaTime > _deltaTimeThreshold) {
			if (Input.GetKey(KeyCode.V)) {//CREATE equiped device
				int randomID = Random.Range(0, 12000);
				DeviceInfo deviceInfo = new DeviceInfo(
					randomID,
					"testDevice",
					"pLac",
					10.0f,
					1.0f,
					"![0.8,2]LacI",
					"GFP",
					1.0f
					);
	        	addDevice(0, deviceInfo, DevicesDisplayer.DeviceType.Equiped);
				
			}
			
			if (Input.GetKey(KeyCode.B)) {//CREATE inventory device
				
				int randomID = Random.Range(50, 12000);
				DeviceInfo deviceInfo = new DeviceInfo(
					randomID,
					"testDevice",
					"pLac",
					10.0f,
					1.0f,
					"![0.8,2]LacI",
					"GFP",
					1.0f
					);
	        	addDevice(randomID, deviceInfo, DevicesDisplayer.DeviceType.Inventory);
			}
	        if (Input.GetKey(KeyCode.T)) {//REMOVE
				//FIXME
				//TODO
				if( _equipedDevices.Count > 0) {
					int randomIdx = Random.Range(0, _equipedDevices.Count);
					Device randomDevice = _equipedDevices[randomIdx];
		        	removeDevice(randomDevice.getID());
				}
			}
			_timeAtLastFrame = _timeAtCurrentFrame;
		}
	}
}
