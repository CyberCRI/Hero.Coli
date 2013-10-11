using UnityEngine;
using System.Collections.Generic;

public class Inventory : DeviceContainer
{
  public enum AddingFailure {
    NONE,
    SAME_NAME,
    SAME_BRICKS,
    DEFAULT
  }

  private string            _genericDeviceNamePrefix = "device";


  //promoter
  private static float      testbeta = 10.0f;
  private static string     testformula = "![0.8,2]LacI";
  //rbs
  private static float      testrbsFactor = 1.0f;
  //gene
  private static string     testproteinName = DevicesDisplayer.getRandomProteinName();
  //terminator
  private static float      testterminatorFactor = 1.0f;
  

  private Device getTestDevice() {
  
    string randomName = getAvailableDeviceName();
  	Device testDevice = Device.buildDevice(randomName, testbeta, testformula, testrbsFactor, testproteinName, testterminatorFactor);
  	
  	return testDevice;
  }

  public Inventory() {
    Logger.Log("Inventory() done", Logger.Level.TRACE);
  }

  public override void addDevice(Device device) {
    Logger.Log("Inventory::addDevice("+device+"), count before="+_devices.Count);
    Device copy = Device.buildDevice(device);
    string displayerString = _displayer!=null?"name="+_displayer.name:"null";
    Logger.Log("Inventory::addDevice("+device+")"
      +", copy="+copy
      +", count before="+_devices.Count
      +", _devices="+_devices
      +", _displayer="+displayerString
      );
    _devices.Add(copy);
    Logger.Log("Inventory::addDevice _devices.Add(copy); done");
    _displayer.addInventoriedDevice(copy);
    Logger.Log("Inventory::addDevice("+device+"), count after="+_devices.Count);
  }

  public AddingFailure canAddDevice(Device device) {
    Logger.Log("Inventory::canAddDevice("+device+")", Logger.Level.TRACE);
    if (_devices.Exists(d => d.getName() == device.getName())) {
      Logger.Log("Inventory::canAddDevice: AddingFailure.SAME_NAME",Logger.Level.TRACE);
      return AddingFailure.SAME_NAME;
    } else if (_devices.Exists(d => d.hasSameBricks(device))) {
      Logger.Log("Inventory::canAddDevice: AddingFailure.SAME_BRICKS",Logger.Level.TRACE);
      return AddingFailure.SAME_BRICKS;
    } else {
      Logger.Log("Inventory::canAddDevice: AddingFailure.NONE",Logger.Level.TRACE);
      return AddingFailure.NONE;
    }
  }

  public AddingFailure askAddDevice(Device device) {
    AddingFailure failure = canAddDevice(device);
    if(failure == AddingFailure.NONE){
      Logger.Log("Inventory::askAddDevice: AddingFailure.NONE, added device",Logger.Level.INFO);
      addDevice(device);
    }
    return failure;
  }

  public override void removeDevice(Device device) {
    _devices.Remove(device);
    _displayer.removeInventoriedDevice(device);
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

  // Use this for initialization
  new void Start () {
    Logger.Log("Inventory::Start()...", Logger.Level.TRACE);
    base.Start();
   //by default: contains a test device
    _devices = new List<Device>();
    Device device = getTestDevice();
    Logger.Log("Inventory: constructing, calling UpdateData(List("+device.ToString()+"), List(), List())");
    UpdateData(new List<Device>() { device }, new List<Device>(), new List<Device>());
    Logger.Log("Inventory::Start()...done", Logger.Level.TRACE);
  }
}
