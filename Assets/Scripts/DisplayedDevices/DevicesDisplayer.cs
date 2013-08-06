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
  
  public static List<string> devicesNames = new List<string>( new string [] {
   "test1"
   ,"test2"
   ,"test3"
   ,"test4"
   ,"test5"
  });

  public static List<string> proteinNames = new List<string>( new string[] {
    "LacI"
    ,"pLac"
    ,"GFP"
    ,"H"
    ,"O"
    ,"H2O"
  });

	private static Dictionary<string, string> spriteNamesDictionary = new Dictionary<string,string>(){
    {devicesNames[0], spriteNames[0]},
    {devicesNames[1], spriteNames[1]},
    {devicesNames[2], spriteNames[2]},
    {devicesNames[3], spriteNames[3]},
		{devicesNames[4], spriteNames[4]}
	};
	
	public enum DeviceType {
		Equiped,
		Inventoried
	}
	
	private List<DisplayedDevice> _equipedDevices = new List<DisplayedDevice>();
	private List<DisplayedDevice> _inventoriedDevices = new List<DisplayedDevice>();
	
	private float _timeAtLastFrame = 0f;
    private float _timeAtCurrentFrame = 0f;
    private float _deltaTime = 0f;	
	private float _deltaTimeThreshold = 0.5f;
	
	//TODO use real device width
	static private float _height = 54.0f;
	public UIPanel equipPanel;
	public GameObject equipment;
	public GameObject inventory;
	public UIPanel inventoryPanel;
	public GameObject equipedDevice;	
	public GameObject inventoryDevice;

  public Equipment _equipment;
  public Inventory _inventory;
	
	public GUITransitioner transitioner;
	
	//FOR DEBUG
	private string getRandomSprite() {
		int randomIndex = Random.Range(0, spriteNames.Count);
		return spriteNames[randomIndex];
	}

	public string getSpriteName(string deviceName) {
		string fromDico = spriteNamesDictionary[deviceName];
		string res = (fromDico!=null)?fromDico:getRandomSprite();
		Debug.Log("getSpriteName("+deviceName+")="+res+" (fromDico="+fromDico+")");
		return res;
	}





  /*
   *  ADD
   *
   */

	public void addInventoriedDevice(Device device) {
		Debug.Log("addInventoriedDevice("+device+")");
		bool alreadyInventoried = (!_inventoriedDevices.Exists(inventoriedDevice => inventoriedDevice.GetHashCode() == device.GetHashCode())); 
		if(alreadyInventoried) { 
			Vector3 localPosition = getNewPosition(DeviceType.Inventoried);
			UnityEngine.Transform parent = inventoryPanel.transform;
			
			DisplayedDevice newDevice = 
				InventoriedDisplayedDevice.Create (
          DevicesDisplayer.DeviceType.Inventoried,
          parent,
          localPosition,
          device,
          this,
          getSpriteName(device.getName())
        );
			_inventoriedDevices.Add(newDevice);
		} else {
			Debug.Log("addDevice failed: alreadyInventoried="+alreadyInventoried);
		}
	}
	
	public void addEquipedDevice(Device device) {
		Debug.Log("addEquipedDevice("+device.ToString()+")");
		bool alreadyEquiped = (!_equipedDevices.Exists(equipedDevice => equipedDevice._device.GetHashCode() == device.GetHashCode())); 
		if(alreadyEquiped) { 
			Vector3 localPosition = getNewPosition(DeviceType.Equiped);
			UnityEngine.Transform parent = equipPanel.transform;
			
			DisplayedDevice newDevice = 
				EquipedDisplayedDevice.Create (
          DevicesDisplayer.DeviceType.Equiped,
          parent,
          localPosition,
          device,
          this,
          getSpriteName(device.getName())
        );
			_equipedDevices.Add(newDevice);
		} else {
			Debug.Log("addDevice failed: alreadyEquiped="+alreadyEquiped);
		}
	}

  public void askAddEquipedDevice(Device device) {
    _equipment.addDevice(device);
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
			inventoryPanel.gameObject.SetActive(false);
		} else {
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
			if(idx == -1) idx = _inventoriedDevices.Count;
			res = inventoryDevice.transform.localPosition + new Vector3((idx%3)*_height, -(idx/3)*_height, -0.1f);
		}		
		return res;
	}






  /*
   *  REMOVE
   *
   */



  public void askRemoveEquipedDevice(Device device) {
    _equipment.removeDevice(device);
  }
	
	public void removeDevice(int deviceID) {
		DisplayedDevice toRemove = _equipedDevices.Find(device => device.getID() == deviceID);
		List<DisplayedDevice> devices = _equipedDevices;
		DeviceType deviceType = DeviceType.Equiped;
		if(toRemove == null) {
			toRemove = _inventoriedDevices.Find(device => device.getID() == deviceID);
			devices = _inventoriedDevices;
			deviceType = DeviceType.Inventoried;
		} else {
		}

    removeDevice(toRemove, devices, deviceType);
	}

  public void removeEquipedDevice(Device toRemove) {
    removeDevice(DevicesDisplayer.DeviceType.Equiped, toRemove);
  }

  public void removeInventoriedDevice(Device toRemove) {
    removeDevice(DevicesDisplayer.DeviceType.Inventoried, toRemove);
  }

  public void removeDevice(DevicesDisplayer.DeviceType type, Device toRemove) {
    List<DisplayedDevice> devices;
    DisplayedDevice found;
    if(type == DevicesDisplayer.DeviceType.Equiped) {
      devices = _equipedDevices;
    } else {
      devices = _inventoriedDevices;
    }
    found = devices.Find(device => device._device.Equals(toRemove));

    if (found != null) {
      removeDevice(found, devices, type);
    } else {
      Debug.Log("removeDevice(type="+type+", toRemove="+toRemove+") found no matching device");
    }
 }

  private void removeDevice(DisplayedDevice toRemove, List<DisplayedDevice> devices, DeviceType deviceType) {
   if(toRemove != null) {
     int startIndex = devices.IndexOf(toRemove);

     if(deviceType == DeviceType.Equiped) {
       Debug.Log("removeDevice("+toRemove+", devices, "+deviceType+") of index "+startIndex+" from equipment of count "+_equipedDevices.Count);
     } else {
       Debug.Log("removeDevice("+toRemove+", devices, "+deviceType+") of index "+startIndex+" from inventory of count "+_inventoriedDevices.Count);
     }

     devices.Remove(toRemove);
     toRemove.Remove();
     if(deviceType == DeviceType.Equiped) {
       Debug.Log("removeDevice("+toRemove+", devices, "+deviceType+") from equipment of count "+_equipedDevices.Count+" done");
     } else {
       Debug.Log("removeDevice("+toRemove+", devices, "+deviceType+") from inventory of count "+_inventoriedDevices.Count+" done");
     }
     for(int idx = startIndex; idx < devices.Count; idx++) {
       Vector3 newLocalPosition = getNewPosition(deviceType, idx);
       Debug.Log("removeDevice("+toRemove+", devices, "+deviceType+") redrawing idx "+idx+" at position "+newLocalPosition);
       devices[idx].Redraw(newLocalPosition);
     }
   }
  }

  public static string getRandomProteinName() {
    return proteinNames[Random.Range(0, proteinNames.Count)];
  }
	
	// Use this for initialization
	void Start () {
		inventoryPanel.gameObject.SetActive(false);
	}

	
	// Update is called once per frame
	void Update () {
		
		_timeAtCurrentFrame = Time.realtimeSinceStartup;
    _deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
		
		bool update = (_deltaTime > _deltaTimeThreshold);
		
		if(update) {
			if (Input.GetKey(KeyCode.V)) {//CREATE equiped device	
				_timeAtLastFrame = _timeAtCurrentFrame;
				
				Debug.Log("V - create equiped device starting...");
				
				//promoter
				float beta = 10.0f;
				string formula = "![0.8,2]LacI";
				//rbs
				float rbsFactor = 1.0f;
				//gene
				string proteinName = getRandomProteinName();
				//terminator
				float terminatorFactor = 1.0f;

				Device newDevice = Device.buildDevice(null,
				 beta,//promoter
				 formula,//promoter
				 rbsFactor,//rbs
				 proteinName,//gene
				 terminatorFactor//terminator
				);
				if(newDevice == null) {
					Debug.Log("failed to provide device");
				} else {
					_equipment.addDevice(newDevice);
				}
        Debug.Log("V - create equiped device... done!");
								
			}
			
			if (Input.GetKey(KeyCode.B)) {//CREATE inventory device
				_timeAtLastFrame = _timeAtCurrentFrame;
				
				Debug.Log("B - create inventory device starting...");
				
				//promoter
				float beta = 10.0f;
				string formula = "![0.8,2]LacI";
				//rbs
				float rbsFactor = 1.0f;
				//gene
				string proteinName = getRandomProteinName();
				//terminator
				float terminatorFactor = 1.0f;
				Device newDevice = Device.buildDevice(null,
				 beta,//promoter
				 formula,//promoter
				 rbsFactor,//rbs
				 proteinName,//gene
				 terminatorFactor//terminator
				);
       if(newDevice == null) {
          Debug.Log("failed to provide device");
        } else {
			    _inventory.addDevice(newDevice);
        }
        Debug.Log("B - create inventoried device... done!");
			}
	        if (Input.GetKey(KeyCode.T)) {//REMOVE
				_timeAtLastFrame = _timeAtCurrentFrame;
				
				Debug.Log("T - remove random device");
				if( _equipedDevices.Count > 0) {
					int randomIdx = Random.Range(0, _equipedDevices.Count);
					DisplayedDevice randomDevice = _equipedDevices[randomIdx];
		      _equipment.removeDevice(randomDevice._device);
        }
			}
		}
	}
}
