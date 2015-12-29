using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO adapt script for Device/Molecules pair in new interface
public class DevicesDisplayer : MonoBehaviour {


  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "DeviceDisplayer";
  private static DevicesDisplayer _instance;
  public static DevicesDisplayer get() {
    if(_instance == null) {
      Logger.Log("DevicesDisplayer::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<DevicesDisplayer>();
    }
    return _instance;
  }
  void Awake()
  {
    Logger.Log("DevicesDisplayer::Awake", Logger.Level.DEBUG);
    _instance = this;
  }
  ////////////////////////////////////////////////////////////////////////////////////////////

  public enum TextureQuality {
    HIGH,
    NORMAL,
    LOW,
    DEFAULT
  }

  private static TextureQuality textureQuality = TextureQuality.LOW;

  public static TextureQuality getTextureQuality()
  {
    return textureQuality;
  }

  public enum DeviceType {
    Equiped,
    Inventoried,
    Listed
  }
	
  private List<DisplayedDevice> _equipedDevices = new List<DisplayedDevice>();
  private List<DisplayedDevice> _inventoriedDevices = new List<DisplayedDevice>();
  private List<DisplayedDevice> _listedInventoriedDevices = new List<DisplayedDevice>();
	
	//TODO use game object texture dimensions
  static private float _equipedHeight = 0.0f;
  static private float _inventoriedWidth = 68.0f;
  static private float _inventoriedHeight = 68.0f;
  static private float _listedInventoriedHeight = 66.0f;
  static private float _listedInventoriedWidth = 66.0f;

  public UIPanel equipPanel;
  public UIPanel inventoryPanel;
  public UIPanel listedInventoryPanel;

  public GraphMoleculeList graphMoleculeList;

  public GameObject equipedDevice;
  public GameObject equipedDevice2;

  public GameObject inventoryDevice;
  public GameObject listedInventoryDevice;

  private GUITransitioner transitioner;
  private CraftZoneManager craftZoneManager;





  /*
   *  ADD
   *
   */

	public void addInventoriedDevice(Device device) {
		Logger.Log("DevicesDisplayer::addInventoriedDevice("+device+")"
      +" starts with _listedInventoriedDevices="+Logger.ToString<DisplayedDevice>(_listedInventoriedDevices),
      Logger.Level.TRACE);

    if(device==null)
    {
      Logger.Log("DevicesDisplayer::addInventoriedDevice device==null", Logger.Level.WARN);
      return;
    }

    bool alreadyInventoried = (_inventoriedDevices.Exists(inventoriedDevice => inventoriedDevice._device == device));
		if(!alreadyInventoried) {
      // ADD TO EQUIPABLE DEVICES
			Vector3 localPosition = getNewPosition(DeviceType.Inventoried);
			UnityEngine.Transform parent = inventoryPanel.transform;
			
			DisplayedDevice newDevice =
				InventoriedDisplayedDevice.Create(
          parent,
          localPosition,
          null,
          device,
          this,
          DevicesDisplayer.DeviceType.Inventoried
        );
			_inventoriedDevices.Add(newDevice);
            //TODO
      //redraw (_inventoriedDevices, DeviceType.Inventoried);

      // ADD TO LISTED DEVICES
      Logger.Log("DevicesDisplayer::addInventoriedDevice: adding listed device", Logger.Level.TRACE);
      localPosition = getNewPosition(DeviceType.Listed);
      Logger.Log("DevicesDisplayer::addInventoriedDevice: localPosition="+localPosition, Logger.Level.TRACE);
      parent = listedInventoryPanel.transform;
      Logger.Log("DevicesDisplayer::addInventoriedDevice: parent="+parent, Logger.Level.TRACE);
      DisplayedDevice newListedDevice =
        ListedDevice.Create(
          parent
          , localPosition
          , null
          , device
          , this
          , DevicesDisplayer.DeviceType.Listed
        );
      Logger.Log("DevicesDisplayer::addInventoriedDevice: newListedDevice="+newListedDevice, Logger.Level.TRACE);
      _listedInventoriedDevices.Add(newListedDevice);

      /*
      if(_listedInventoriedDevices.Count == 1) {
        Logger.Log("DevicesDisplayer::addInventoriedDevice: only 1 listed device", Logger.Level.);
        CraftZoneManager.get().askSetDevice(device);
      }
      */
      
		} else {
			Logger.Log("DevicesDisplayer::addInventoriedDevice failed: alreadyInventoried="+alreadyInventoried, Logger.Level.WARN);
		}
    Logger.Log("DevicesDisplayer::addInventoriedDevice("+device+")"
      +" end with _listedInventoriedDevices="+Logger.ToString<DisplayedDevice>(_listedInventoriedDevices),
      Logger.Level.TRACE);
	}
	
	public void addEquipedDevice(Device device) {
		Logger.Log("addEquipedDevice("+device.ToString()+")", Logger.Level.TRACE);
    if(device == null)
    {
      Logger.Log ("DevicesDisplayer::addEquipedDevice device == null", Logger.Level.WARN);
    }
		bool newEquiped = (!_equipedDevices.Exists(equiped => equiped._device == device)); 
		if(newEquiped) { 
			Vector3 localPosition = getNewPosition(DeviceType.Equiped);
			UnityEngine.Transform parent = equipPanel.transform;

			DisplayedDevice newDevice = 
				EquipedDisplayedDevice.Create(
          parent,
          localPosition,
          null,
          device,
          this,
          DevicesDisplayer.DeviceType.Equiped
        );
			_equipedDevices.Add(newDevice);

      graphMoleculeList.addDeviceAndMoleculesComponent(newDevice);

		} else {
			Logger.Log("addDevice failed: alreadyEquiped="+newEquiped, Logger.Level.TRACE);
		}
	}

  public DeviceContainer.AddingResult askAddEquipedDevice(Device device) {
    if(device == null)
    {
      Logger.Log("DevicesDisplayer::askAddEquipedDevice device==null", Logger.Level.WARN);
      return DeviceContainer.AddingResult.FAILURE_DEFAULT;
    }
    return Equipment.get().askAddDevice(device);
  }






	public bool IsScreen(int screen) {
    SafeGetTransitioner();
		return (((transitioner._currentScreen == GUITransitioner.GameScreen.screen1) && (screen == 1))
			|| ((transitioner._currentScreen == GUITransitioner.GameScreen.screen2) && (screen == 2))
			|| ((transitioner._currentScreen == GUITransitioner.GameScreen.screen3) && (screen == 3)));
	}
	
	public bool IsEquipScreen() {
		return (SafeGetTransitioner()._currentScreen == GUITransitioner.GameScreen.screen2);
	}
	
	public void UpdateScreen(){
		Logger.Log("DevicesDisplayer::UpdateScreen " + SafeGetTransitioner()._currentScreen, Logger.Level.TRACE);
		if(IsScreen(1)) {
			inventoryPanel.gameObject.SetActive(false);
      listedInventoryPanel.gameObject.SetActive(false);
		} else if(IsScreen(2)) {
      inventoryPanel.gameObject.SetActive(true);
      listedInventoryPanel.gameObject.SetActive(false);
    } else if(IsScreen(3)) {
			inventoryPanel.gameObject.SetActive(false);
      listedInventoryPanel.gameObject.SetActive(true);
		} else {
      Logger.Log("DevicesDisplayer::UpdateScreen unknown current screen", Logger.Level.WARN);
    }
	}
 
  public Vector3 getNewPosition(DeviceType deviceType, int index = -1) {
    Vector3 res;
    int idx = index;
    if(deviceType == DeviceType.Equiped) {
      if(idx == -1) idx = _equipedDevices.Count;
      res = equipedDevice.transform.localPosition + new Vector3(0.0f, -idx*_equipedHeight, -0.1f);
    }
    else if(deviceType == DeviceType.Inventoried)
    {
            //height limit: 6 devices
            //width limit: 3 devices
            int initialIdx = idx;
      if(idx == -1) idx = _inventoriedDevices.Count;
            //TODO
            /*
            int columnIdx = idx/6;
            int columnCount = _inventoriedDevices.Count/6;
            //columnCount = 1 & columnIdx = 0 => 0
            //columnCount = 2 & columnIdx = 0 => -0.5
            //columnCount = 2 & columnIdx = 1 => +0.5
            //columnCount = 3 & columnIdx = 0 => -1
            //columnCount = 3 & columnIdx = 1 => 0
            //columnCount = 3 & columnIdx = 2 => +1
            //columnCount = 4 & columnIdx = 0 => -1.5
            //columnCount = 4 & columnIdx = 1 => -0.5
            //columnCount = 4 & columnIdx = 2 => +0.5
            //columnCount = 4 & columnIdx = 3 => +1.5
            //halfIntervals = columnCount*0.5
            //shift = halfIntervals + columnIdx
            float halfIntervals = -columnCount*0.5f;
            float shift = halfIntervals + columnIdx;
            res = inventoryDevice.transform.localPosition + new Vector3(shift*_inventoriedWidth, -(idx%6)*_inventoriedHeight, -0.1f);
            Logger.Log("initialIdx="+initialIdx+",idx="+idx+",columnIdx="+columnIdx+",columnCount="+columnCount+",halfIntervals="+halfIntervals+",shift="+shift, Logger.Level.ERROR);
            */
            res = inventoryDevice.transform.localPosition + new Vector3((idx%2-0.5f)*_inventoriedWidth, -(idx/2)*_inventoriedHeight, -0.1f);
    }
    else if(deviceType == DeviceType.Listed)
    {
      if(idx == -1) idx = _listedInventoriedDevices.Count;
      res = listedInventoryDevice.transform.localPosition + new Vector3((idx%4)*_listedInventoriedWidth, -(idx/4)*_listedInventoriedHeight, -0.1f);

      Logger.Log ("DevicesDisplayer::getNewPosition type=="+deviceType
        +", idx="+idx
        +", localPosition="+listedInventoryDevice.transform.localPosition
        +", res="+res
        );
    }
    else
    {
      Logger.Log("DevicesDisplayer::getNewPosition: Error: unmanaged type "+deviceType, Logger.Level.WARN);
      res = new Vector3();
    }
    return res;
 }






  /*
   *  REMOVE
   *
   */



  public bool askRemoveEquipedDevice(Device device) {
    Equipment.get().removeDevice(device);
    return true;
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
    graphMoleculeList.removeDeviceAndMoleculesComponent(toRemove);
  }

  public void removeInventoriedDevice(Device toRemove) {
    removeDevice(DevicesDisplayer.DeviceType.Inventoried, toRemove);
  }

    public void removeListedDevice(Device toRemove) {
        removeDevice(DevicesDisplayer.DeviceType.Listed, toRemove);    
    }

    public void removeDevice (DevicesDisplayer.DeviceType type, Device toRemove)
    {
        List<DisplayedDevice> devices;
        DisplayedDevice found;
        if (type == DevicesDisplayer.DeviceType.Equiped) {
            devices = _equipedDevices;
        } else if (type == DevicesDisplayer.DeviceType.Inventoried) {
            devices = _inventoriedDevices;
        } else if (type == DevicesDisplayer.DeviceType.Listed) {
            devices = _listedInventoriedDevices;
        } else {
            Logger.Log ("DevicesDisplayer::removeDevice unknown type="+type, Logger.Level.WARN);
            devices = new List<DisplayedDevice>();
        }
        found = devices.Find (device => device._device.Equals (toRemove));

        if (found != null) {
            removeDevice (found, devices, type);
        } else {
            Logger.Log ("removeDevice(type=" + type + ", toRemove=" + toRemove + ") found no matching device", Logger.Level.WARN);
        }
    }

    private void removeDevice (DisplayedDevice toRemove, List<DisplayedDevice> devices, DeviceType deviceType)
    {
        if (toRemove != null) {
            int startIndex = devices.IndexOf (toRemove);

            //debug
            int count = deviceType == DeviceType.Equiped ? _equipedDevices.Count : (deviceType == DeviceType.Inventoried ? _inventoriedDevices.Count : _listedInventoriedDevices.Count);
            string source = deviceType == DeviceType.Equiped ? "equipment" : (deviceType == DeviceType.Inventoried ? "inventory" : "listed");
            Logger.Log ("removeDevice(" + toRemove + ", devices, " + deviceType + ") of index " + startIndex + " from " + source + " of count " + count, Logger.Level.TRACE);

            devices.Remove (toRemove);
            toRemove.Remove ();

            //debug
            count = deviceType == DeviceType.Equiped ? _equipedDevices.Count : (deviceType == DeviceType.Inventoried ? _inventoriedDevices.Count : _listedInventoriedDevices.Count);
            Logger.Log ("removeDevice(" + toRemove + ", devices, " + deviceType + ") from "+source+" of count " + count + " done", Logger.Level.TRACE);

            redraw (devices, deviceType, startIndex);
        }
    }

    private void redraw(List<DisplayedDevice> devices, DeviceType deviceType, int startIndex = 0)
    {
        //redraw
        for (int idx = startIndex; idx < devices.Count; idx++) {
            Vector3 newLocalPosition = getNewPosition (deviceType, idx);
            devices [idx].Redraw (newLocalPosition);
        }
    }

  private GUITransitioner SafeGetTransitioner()
  {
    if(null == transitioner)
    {
      transitioner = GUITransitioner.get();
    }
    return transitioner;
  }

  void Start () {
	  Logger.Log("DevicesDisplayer::Start()", Logger.Level.DEBUG);

    SafeGetTransitioner();
	  inventoryPanel.gameObject.SetActive(false);
    if(null == equipedDevice) {
      Logger.Log("DevicesDisplayer::Start null==equipedDevice", Logger.Level.WARN);
            
      equipedDevice = GameObject.Find("InterfaceLinkManager").GetComponent<InterfaceLinkManager>().equipedDevice;
      equipedDevice2 = GameObject.Find("InterfaceLinkManager").GetComponent<InterfaceLinkManager>().equipedDevice2;
    }
    if(_equipedHeight == 0.0f)
    {
      _equipedHeight = equipedDevice.transform.localPosition.y - equipedDevice2.transform.localPosition.y;
    }
    equipedDevice.SetActive(false);
    equipedDevice2.SetActive(false);
    inventoryDevice.SetActive (false);
    if(null == equipPanel)
    {
      equipPanel = GameObject.Find("InterfaceLinkManager").GetComponent<InterfaceLinkManager>().equipedDevicesSlotsPanel;
    }
    equipPanel.gameObject.SetActive(false);
  }
}
