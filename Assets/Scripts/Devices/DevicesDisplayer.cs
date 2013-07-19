using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DevicesDisplayer : MonoBehaviour {
	
	private List<Device> _devices = new List<Device>();
	public GameObject _devicePrefab;
	private float _timeCounter;
	private float _timeDelta = 0.2f;
	
	//TODO use real device width
	static private float _height = 45.0f;
	static private Vector3 _positionOffset = new Vector3(10.0f, 75.0f, 0.0f);
	
	public ReactionEngine reactionEngine;
	public int celliaMediumID = 1;
	
	
	public void addDevice(int deviceID, DeviceInfo deviceInfo) {
		Debug.Log("addDevice("+deviceID+", "+deviceInfo+")");
		if(!_devices.Exists(device => device.getID() == deviceID)) { 
			Vector3 localPosition = _positionOffset + new Vector3(0.0f, -_devices.Count*_height, 0.0f);
			Device device = Device.Create (gameObject.transform, localPosition, deviceID);
			_devices.Add(device);
			
			//let's add reaction to reaction engine
			//for each module of deviceInfo, add to reaction engine
			deviceInfo._modules.ForEach( module => module.addToReactionEngine(celliaMediumID, reactionEngine));
		}
	}
	
	
	
	public void removeDevice(int deviceID) {
		Debug.Log("removeDevice("+deviceID+")");
		Device toRemove = _devices.Find(device => device.getID() == deviceID);
		if(toRemove != null) {
			_devices.Remove(toRemove);
			toRemove.Remove();
			for(int i = 0; i < _devices.Count; i++) {
				Vector3 newLocalPosition = _positionOffset + new Vector3(0.0f, -i*_height, 0.0f);
				_devices[i].Redraw(newLocalPosition);
			}
		}
	}
	
	// Use this for initialization
	void Start () {		
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
				LinkedList<Product> products = new LinkedList<Product>();
				products.AddLast(new Product("GFP",1.0f));
				ModuleInfo moduleInfo = new ModuleInfo(
					"pLac",
					10.0f,
					1.0f,
					"![0.8,2]LacI",
					products
					);
				List<ModuleInfo> modules = new List<ModuleInfo>();
				modules.Add(moduleInfo);
				DeviceInfo deviceInfo = new DeviceInfo(
					randomID,
					"testDevice",
					modules
					);
	        	addDevice(randomID, deviceInfo);
			}
	        if (Input.GetKey(KeyCode.T)) {//REMOVE
				if(_devices.Count > 0) {
					int randomIdx = Random.Range(0, _devices.Count);
					Device randomDevice = _devices[randomIdx];
		        	removeDevice(randomDevice.getID());
				}
			}
			_timeCounter = Time.time;
		}
	}
}
