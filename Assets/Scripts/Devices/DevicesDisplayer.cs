using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DevicesDisplayer : MonoBehaviour {
	
	private List<Device> _devices = new List<Device>();
	private int equipedDevicesCount = 0;
	
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
	public void addDevice(int deviceID, DeviceInfo deviceInfo, bool isEquiped) {
		if(isEquiped){
			Debug.Log("addDevice("+deviceID+", "+deviceInfo+")");
			if(!_devices.Exists(device => device.getID() == deviceID)) { 
				Vector3 localPosition = equipedDevice.transform.localPosition + new Vector3(0.0f, -equipedDevicesCount*_height, -0.1f);
				Device device = Device.Create (equipPanel.transform, localPosition, deviceID);
				_devices.Add(device);
			}
		} else {			
			Debug.Log("addDevice("+deviceID+") in inventory");
			if(!_devices.Exists(device => device.getID() == deviceID)) { 
				Vector3 localPosition = inventoryDevice.transform.localPosition + new Vector3((_devices.Count%3)*_height, -(_devices.Count/3)*_height, -0.1f);
				Device device = Device.Create (inventoryPanel.transform, localPosition, deviceID);
				_devices.Add(device);
			}
		}
			
		//let's add reaction to reaction engine
		//for each module of deviceInfo, add to reaction engine
		//deviceInfo._modules.ForEach( module => module.addToReactionEngine(celliaMediumID, reactionEngine));
		
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
	
	public void removeDevice(int deviceID) {
		Debug.Log("removeDevice("+deviceID+")");
		Device toRemove = _devices.Find(device => device.getID() == deviceID);
		if(toRemove != null) {
			_devices.Remove(toRemove);
			toRemove.Remove();
			for(int i = 0; i < equipedDevicesCount; i++) {
				Vector3 newLocalPosition = equipedDevice.transform.localPosition + new Vector3(0.0f, -i*_height, 0.0f);
				_devices[i].Redraw(newLocalPosition);
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
	        	addDevice(randomID, deviceInfo, true);
				equipedDevicesCount++;
				
			}
			
			if (Input.GetKey(KeyCode.B)) {//CREATE inventory device
				
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
	        	addDevice(randomID, deviceInfo, false);
			}
	        if (Input.GetKey(KeyCode.T)) {//REMOVE
				if( equipedDevicesCount > 0) {
					int randomIdx = Random.Range(0, _devices.Count);
					Device randomDevice = _devices[randomIdx];
		        	removeDevice(randomDevice.getID());
					equipedDevicesCount--;
				}
			}
			_timeAtLastFrame = _timeAtCurrentFrame;
		}
	}
}
