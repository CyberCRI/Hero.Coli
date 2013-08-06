using UnityEngine;
using System.Collections.Generic;

public class Inventory : DeviceContainer
{
					
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
    _devices.Add(device);
    _displayer.addInventoriedDevice(device);
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
