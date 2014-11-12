using UnityEngine;
using System.Collections.Generic;

public class Inventory : DeviceContainer
{

  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "DeviceInventory";
	public InventoryAnimator animator;
  private static Inventory _instance;
  public static Inventory get() {
    if(_instance == null) {
      Logger.Log("Inventory::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<Inventory>();
    }
    return _instance;
  }

  public static bool isOpenable()
  {
    return 0 != _instance._devices.Count;
  }

  void Awake()
  {
    Logger.Log("Inventory::Awake", Logger.Level.DEBUG);
		_deviceAdded = false;
    _instance = this;
  }
  ////////////////////////////////////////////////////////////////////////////////////////////

  /* array of file paths from which the devices available by default from start will be loaded */

  //private string[] _deviceFiles = new string[]{};
  private string[] _deviceFiles = new string[]{ "Parameters/Devices/available"};
  //private string[] _deviceFiles = new string[]{ "Assets/Data/devices"};
  //private string[] _deviceFiles = new string[]{ "Assets/Data/raph/devices", Inventory._saveFilePath };
  //private string[] _deviceFiles = new string[]{ "Assets/Data/raph/repressilatorDevices", Inventory._saveFilePath };
	
  private string _saveFilePath = "Assets/Resources/Parameters/Devices/exported.txt";

  private string _genericDeviceNamePrefix = "device";

	private bool _deviceAdded;

	public bool getDeviceAdded() {return _deviceAdded;}
	public void setDeviceAdded(bool b) {_deviceAdded = b;}


  private static string[] proteinsIn = new string[]{
    "Collagen",
    "Actin",
    "Keratin",
    "Elastin"
  };
  private static string[] proteinsOut = new string[]{
    "Cadherin",
    "Ependymin",
    "Integrin",
    "Selectin"
  };

  private static float getTestBeta() { return Random.Range(0.0f, 10.0f); }
  private static string getTestFormula() {
    string result = "";
    if(Random.Range(0.0f, 1.0f) > 0.5f){
      result += "!";
    }
    result += "[";
    result += (0.1f + Random.Range(0, 8)/10);
    result += ",";
    result += Random.Range (0.1f, 3.0f);
    result += "]";
    result += proteinsIn[Random.Range(0, proteinsIn.Length-1)];
    return result;
  }
  private static float getTestRBS() { return Random.Range(0.9f, 1.1f); }
  private static string getTestProtein() { return proteinsOut[Random.Range(0, proteinsOut.Length)]; }
  private static float getTestTerminator() { return Random.Range(0.9f, 1.1f); }

  private Device getTestDevice(string name) {
    Logger.Log("Inventory::getTestDevice()", Logger.Level.TRACE);
  	Device testDevice = Device.buildDevice(
      name,
      getTestBeta(),
      getTestFormula(),
      getTestRBS(),
      getTestProtein(),
      getTestTerminator()
      );
  	Logger.Log("Inventory::getTestDevice() testDevice="+testDevice, Logger.Level.TRACE);
  	return testDevice;
  }

  private List<Device> getTestDevices() {
    Logger.Log("Inventory::getTestDevices()", Logger.Level.TRACE);
    List<Device> result = new List<Device>();
    result.Add(getTestDevice("MyDevice"));
    result.Add(getTestDevice("MonDispositif"));
    result.Add(getTestDevice("MeinGeraet"));
    Logger.Log("Inventory::getTestDevices() result="+Logger.ToString<Device>(result), Logger.Level.TRACE);
    return result;
  }

  public Inventory() {
    Logger.Log("Inventory() done", Logger.Level.TRACE);
  }

  private void addDevice(Device device) {
    Logger.Log("Inventory::addDevice("+device+"), count before="+_devices.Count, Logger.Level.TRACE);
    Device copy = Device.buildDevice(device);
    if(device==null)
    {
      Logger.Log("Inventory::addDevice device==null", Logger.Level.WARN);
      return;
    }

    string displayerString = _displayer!=null?"name="+_displayer.name:"null";
    Logger.Log("Inventory::addDevice("+device+")"
      +", copy="+copy
      +", count before="+_devices.Count
      +", _devices="+_devices
      +", _displayer="+displayerString
      , Logger.Level.TRACE
      );
    _devices.Add(copy);
    Logger.Log("Inventory::addDevice _devices.Add(copy); done", Logger.Level.TRACE);
    _displayer.addInventoriedDevice(copy);
    Logger.Log("Inventory::addDevice("+device+"), count after="+_devices.Count, Logger.Level.TRACE);

		if (animator.isPlaying ==false)
		{
			_deviceAdded = true;
			animator.Play();
		}
  }

  public AddingResult canAddDevice(Device device) {
    Logger.Log("Inventory::canAddDevice("+device+") with _devices="+Logger.ToString<Device>(_devices), Logger.Level.TRACE);

    if(device == null) {
      Logger.Log("Inventory::canAddDevice: device is null: AddingResult.FAILURE_DEFAULT",Logger.Level.WARN);
      return AddingResult.FAILURE_DEFAULT;
    } else {
      if (_devices.Exists(d => d.getName() == device.getName())) {
        if (_devices.Exists(d => d.hasSameBricks(device))) {
          Logger.Log("Inventory::canAddDevice: AddingResult.FAILURE_SAME_DEVICE",Logger.Level.TRACE);
          return AddingResult.FAILURE_SAME_DEVICE;
        } else {
          Logger.Log("Inventory::canAddDevice: AddingResult.FAILURE_SAME_NAME",Logger.Level.TRACE);
          return AddingResult.FAILURE_SAME_NAME;
        }
      } else if (_devices.Exists(d => d.hasSameBricks(device))) {
        Logger.Log("Inventory::canAddDevice: AddingResult.FAILURE_SAME_BRICKS",Logger.Level.TRACE);
        return AddingResult.FAILURE_SAME_BRICKS;
      } else {
        Logger.Log("Inventory::canAddDevice: AddingResult.SUCCESS",Logger.Level.TRACE);
        return AddingResult.SUCCESS;
      }
    }
  }

  public override AddingResult askAddDevice(Device device) {
    Logger.Log("Inventory::askAddDevice",Logger.Level.TRACE);
    AddingResult addingResult = canAddDevice(device);
    if(addingResult == AddingResult.SUCCESS){
      Logger.Log("Inventory::askAddDevice: AddingResult.SUCCESS, will add device="+device,Logger.Level.INFO);
      addDevice(device);

      //DeviceSaver dSaver = new DeviceSaver();
      //dSaver.saveDevicesToFile(_devices, _saveFilePath);
    } else {
      Logger.Log("Inventory::askAddDevice: "+addingResult+", didn't add device="+device,Logger.Level.INFO);
    }
    return addingResult;
  }

  public override void removeDevice(Device device) {
    _devices.Remove(device);
    safeGetDisplayer().removeInventoriedDevice(device);
  }

  public override void editDevice(Device device) {
    //TODO
    Debug.Log("Inventory::editeDevice NOT IMPLEMENTED");
  }

  public string getAvailableDeviceName() {
    Logger.Log("Inventory::getAvailableDeviceName()", Logger.Level.TRACE);
    bool taken;
    string currentName;
    int number = _devices.Count;
    do {
      currentName = _genericDeviceNamePrefix+number;
      taken = _devices.Exists(d => (d.getName() == currentName));
      if(taken){
        number++;
      } else {
        return currentName;
      }
    } while (taken);
    Logger.Log("Inventory::getAvailableDeviceName() returns "+currentName, Logger.Level.TRACE);
    return currentName;
  }
	
  void loadDevices() {
	LinkedList<BioBrick> availableBioBricks = AvailableBioBricksManager.get().getAvailableBioBricks();
    List<Device> devices = new List<Device>();

    DeviceLoader dLoader = new DeviceLoader(availableBioBricks);
    foreach (string file in _deviceFiles) {
      Logger.Log("Inventory::loadDevices loads device file "+file, Logger.Level.TRACE);
      devices.AddRange(dLoader.loadDevicesFromFile(file));
    }
    Logger.Log("Inventory::loadDevices calls inventory.UpdateData(List("
			+Logger.ToString<Device>(devices)+"), List(), List())", Logger.Level.TRACE);
    UpdateData(devices, new List<Device>(), new List<Device>());
  }
	
  protected override void Start() {
    base.Start();
    Logger.Log("Inventory::Start()", Logger.Level.DEBUG);
    loadDevices();
  }
}
