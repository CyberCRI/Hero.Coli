using UnityEngine;
using System.Collections.Generic;

public class Equipment : DeviceContainer
{
	public Equipment() {
		//by default, nothing's equiped
		_devices = new List<Device>();
	}

 public override void addDevice(Device device) {
      _devices.Add(device);
      _displayer.addEquipedDevice(device);
 }

 public override void removeDevice(Device device) {
      _devices.Remove(device);
      _displayer.removeEquipedDevice(device);
 }

  public override void editDevice(Device device) {
    //TODO
    Debug.Log("Equipment::editeDevice NOT IMPLEMENTED");
  }
}

