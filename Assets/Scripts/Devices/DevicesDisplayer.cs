using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DevicesDisplayer : MonoBehaviour {
	
	//FOR DEBUG
	private static List<string> spriteNames = new List<string>( new string [] {
		"Backdrop"
		,"brick"
		,"brickNM"
		,"burlap"
		,"sand"
	});
	
	public enum DeviceType {
		Equiped,
		Inventory
	}
	
	private List<DisplayedDevice> _equipedDevices = new List<DisplayedDevice>();
	private List<DisplayedDevice> _inventoryDevices = new List<DisplayedDevice>();
	
	private float _timeAtLastFrame = 0f;
    private float _timeAtCurrentFrame = 0f;
    private float _deltaTime = 0f;	
	private float _deltaTimeThreshold = 0.2f;
	
	//TODO use real device width
	static private float _height = 54.0f;
	public ReactionEngine reactionEngine;
	public int celliaMediumID = 1;
	public UIPanel equipPanel;
	public GameObject equipment;
	public GameObject inventory;
	public UIPanel inventoryPanel;
	public GameObject equipedDevice;	
	public GameObject inventoryDevice;
	
	public GUITransitioner transitioner;
	
	//FOR DEBUG
	private string getRandomSprite() {
		int randomIndex = Random.Range(0, spriteNames.Count);
		return spriteNames[randomIndex];
	}
	
	public void addDevice(int deviceID, DeviceInfo deviceInfo, DeviceType deviceType) {
		
		Debug.Log("addDevice("+deviceID+", "+deviceInfo+", "+deviceType+")");
		bool alreadyEquiped = (!_equipedDevices.Exists(device => device.getID() == deviceID));
		bool alreadyInventory = (!_inventoryDevices.Exists(device => device.getID() == deviceID)); 
		if(alreadyEquiped || alreadyInventory) { 
			Vector3 localPosition;
			UnityEngine.Transform parent;
			List<DisplayedDevice> devices;
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
			DisplayedDevice newDevice = DisplayedDevice.Create (parent, localPosition, newDeviceId, deviceType, deviceInfo, this);
			devices.Add(newDevice);
			//let's add reaction to reaction engine
			//for each module of deviceInfo, add to reaction engine
			//deviceInfo._modules.ForEach( module => module.addToReactionEngine(celliaMediumID, reactionEngine));
		} else {
			Debug.Log("addDevice failed: alreadyEquiped="+alreadyEquiped+", alreadyInventory="+alreadyInventory);
		}
	}
	
	public bool IsScreen(int screen) {
		return (((transitioner._currentScreen == GUITransitioner.GameScreen.screen1) && (screen == 1))
			|| ((transitioner._currentScreen == GUITransitioner.GameScreen.screen2) && (screen == 2))
			|| ((transitioner._currentScreen == GUITransitioner.GameScreen.screen3) && (screen == 3)));
	}
	
	public bool IsEquipScreen() {
		return (transitioner._currentScreen == GUITransitioner.GameScreen.screen2);
	}
	
	public void UpdateScreen(){
		Debug.Log("UpdateScreen " + transitioner._currentScreen);
		if(IsScreen(1) || IsScreen(3)){
			//inventoryPanel.SetAlphaRecursive(0,true);
			inventoryPanel.gameObject.SetActive(false);
		} else {
		//if(IsScreen(2)){
			//inventoryPanel.SetAlphaRecursive(100,true);
			inventoryPanel.gameObject.SetActive(true);
		}
	}
	
	public Vector3 getNewPosition(DeviceType deviceType, int index = -1) {
		Vector3 res;
		int idx = index;
		if(deviceType == DeviceType.Equiped) {
			if(idx == -1) idx = _equipedDevices.Count;
			res = equipedDevice.transform.localPosition + new Vector3(0.0f, -idx*_height, -0.1f);
		} else {
			if(idx == -1) idx = _inventoryDevices.Count;
			res = inventoryDevice.transform.localPosition + new Vector3((idx%3)*_height, -(idx/3)*_height, -0.1f);
		}		
		return res;
	}
	
	public void removeDevice(int deviceID) {
		DisplayedDevice toRemove = _equipedDevices.Find(device => device.getID() == deviceID);
		List<DisplayedDevice> devices = _equipedDevices;
		DeviceType deviceType = DeviceType.Equiped;
		if(toRemove == null) {
			toRemove = _inventoryDevices.Find(device => device.getID() == deviceID);
			devices = _inventoryDevices;
			deviceType = DeviceType.Inventory;
		} else {
		}
		
		if(toRemove != null) {
			int startIndex = devices.IndexOf(toRemove);
			
			if(deviceType == DeviceType.Equiped) {
				Debug.Log("removeDevice("+deviceID+") of index "+startIndex+" from equipment of count "+_equipedDevices.Count);
			} else {
				Debug.Log("removeDevice("+deviceID+") of index "+startIndex+" from inventory of count "+_inventoryDevices.Count);
			}
			
			devices.Remove(toRemove);
			toRemove.Remove();
			if(deviceType == DeviceType.Equiped) {
				Debug.Log("removeDevice("+deviceID+") from equipment of count "+_equipedDevices.Count+" done");
			} else {
				Debug.Log("removeDevice("+deviceID+") from inventory of count "+_inventoryDevices.Count+" done");
			}
			for(int idx = startIndex; idx < devices.Count; idx++) {
				Vector3 newLocalPosition = getNewPosition(deviceType, idx);
				Debug.Log("removeDevice("+deviceID+") redrawing idx "+idx+" at position "+newLocalPosition);
				devices[idx].Redraw(newLocalPosition);
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
				string spriteName = getRandomSprite();
				
				DeviceInfo deviceInfo = new DeviceInfo(
					randomID,
					"testDevice",
					spriteName,
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
				string spriteName = getRandomSprite();
				DeviceInfo deviceInfo = new DeviceInfo(
					randomID,
					"testDevice",
					spriteName,
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
					DisplayedDevice randomDevice = _equipedDevices[randomIdx];
		        	removeDevice(randomDevice.getID());
				}
			}
			_timeAtLastFrame = _timeAtCurrentFrame;
		}
	}
}
