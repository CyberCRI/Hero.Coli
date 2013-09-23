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
					
  //promoter
  private static float testbeta = 10.0f;
  private static string testformula = "![0.8,2]LacI";
  //rbs
  private static float testrbsFactor = 1.0f;
  //gene
  private static string testproteinName = DevicesDisplayer.getRandomProteinName();
  //terminator
  private static float testterminatorFactor = 1.0f;
  
  	
  private static Device getTestDevice() {
  
    string randomName = DevicesDisplayer.devicesNames[Random.Range (0, DevicesDisplayer.devicesNames.Count)];
  	Device testDevice = Device.buildDevice(randomName, testbeta, testformula, testrbsFactor, testproteinName, testterminatorFactor);
  	
  	return testDevice;
  }

  public Inventory() {
  	//by default: contains a test device
    _devices = new List<Device>();
    Device device = getTestDevice();
    Debug.Log("Inventory: constructing, calling UpdateData(List("+device.ToString()+"), List(), List())");
  	UpdateData(new List<Device>() { device }, new List<Device>(), new List<Device>());
  }

  public override void addDevice(Device device) {
    Device copy = Device.buildDevice(device);
    _devices.Add(copy);
    _displayer.addInventoriedDevice(copy);
  }

  public AddingFailure askAddDevice(Device device) {
    if (_devices.Exists(d => d.getName() == device.getName())) {
      Logger.Log("Inventory::askAddDevice: AddingFailure.SAME_NAME");
      return AddingFailure.SAME_NAME;
    } else if (_devices.Exists(d => d.hasSameBricks(device))) {
      Logger.Log("Inventory::askAddDevice: AddingFailure.SAME_BRICKS");
      return AddingFailure.SAME_BRICKS;
    } else {
      Logger.Log("Inventory::askAddDevice: AddingFailure.NONE");
      addDevice(device);
      return AddingFailure.NONE;
    }
  }

  public override void removeDevice(Device device) {
    _devices.Remove(device);
    _displayer.removeInventoriedDevice(device);
  }

  public override void editDevice(Device device) {
    //TODO
    Debug.Log("Inventory::editeDevice NOT IMPLEMENTED");
  }
}
