using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DevicesDisplayer : MonoBehaviour {
	
	private List<Device> _devices = new List<Device>();
	private int equipedDevicesCount = 0;
	public GameObject _devicePrefab;
	private float _timeCounter;
	private float _timeDelta = 0.1f;
	//TODO use real device width
	static private float _height = 45.0f;
	public ReactionEngine reactionEngine;
	public int celliaMediumID = 1;
	public UIPanel equipPanel;
	public UIPanel catalogPanel;
	public UISprite equipPanelSlotPosition;	
	public UISprite catalogPanelSlotPosition;
	public UIPanel catalogPanelOffset;
	public UIPanel equipPanelOffset;
	public void addDevice(int deviceID, DeviceInfo deviceInfo, bool isEquiped) {
		if(isEquiped){
			Debug.Log("addDevice("+deviceID+", "+deviceInfo+")");
			if(!_devices.Exists(device => device.getID() == deviceID)) { 
				Vector3 localPosition = equipPanelSlotPosition.transform.localPosition;//+ new Vector3(0.0f, -equipedDevicesCount*_height, 0.0f);
				Device device = Device.Create (equipPanelOffset.transform, localPosition, deviceID);
				_devices.Add(device);
			}
		}
		else{
			
			Debug.Log("addDevice("+deviceID+") in catalog");
				if(!_devices.Exists(device => device.getID() == deviceID)) { 
					Vector3 localPosition = catalogPanelSlotPosition.transform.localPosition; //+ new Vector3((_devices.Count%3)*_height, -(_devices.Count/3)*_height, 0.0f);
					Device device = Device.Create (catalogPanelOffset.transform, localPosition, deviceID);
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
			catalogPanel.SetAlphaRecursive(0,true);
		}
		if(screenID == 2){
			catalogPanel.SetAlphaRecursive(100,true);
		}
	}
	
	public void removeDevice(int deviceID) {
		Debug.Log("removeDevice("+deviceID+")");
		Device toRemove = _devices.Find(device => device.getID() == deviceID);
		if(toRemove != null) {
			_devices.Remove(toRemove);
			toRemove.Remove();
			for(int i = 0; i < equipedDevicesCount; i++) {
				Vector3 newLocalPosition = equipPanelSlotPosition.transform.localPosition + new Vector3(0.0f, -i*_height, 0.0f);
				_devices[i].Redraw(newLocalPosition);
			}
		}
	}
	
	// Use this for initialization
	void Start () {		
		catalogPanel.SetAlphaRecursive(0,true);//screen 1 at the beginning = no catalog
		/*
		for(int i = 0; i < 5; i++) {
			addDevice (i);
		}
		*/
	}

	
	// Update is called once per frame
	void Update () {
		if(Time.time - _timeCounter > _timeDelta) {
			if (Input.GetKey(KeyCode.V)) {//CREATE
				int randomID = Random.Range(0, 12000);
				ModuleInfo moduleInfo = new ModuleInfo(
					"pLac",
					10.0f,
					1.0f,
					"![0.8,2]LacI",
					"GFP",
					1.0f);
				List<ModuleInfo> modules = new List<ModuleInfo>();
				modules.Add(moduleInfo);
				DeviceInfo deviceInfo = new DeviceInfo(
					randomID,
					"testDevice",
					modules
					);
	        	addDevice(randomID, deviceInfo, true);
				equipedDevicesCount++;
				
			}
			
			if (Input.GetKey(KeyCode.B)) {//CREATE
				
				int randomID = Random.Range(0, 12000);
				ModuleInfo moduleInfo = new ModuleInfo(
					"pLac",
					10.0f,
					1.0f,
					"![0.8,2]LacI",
					"GFP",
					1.0f);
				List<ModuleInfo> modules = new List<ModuleInfo>();
				modules.Add(moduleInfo);
				DeviceInfo deviceInfo = new DeviceInfo(
					randomID,
					"testDevice",
					modules
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
			_timeCounter = Time.time;
		}
	}
}
