using UnityEngine;
using System.Collections.Generic;

public abstract class DeviceContainer : MonoBehaviour {
	
  protected List<Device> _devices;

  public DevicesDisplayer _displayer;
	
  public void UpdateData(List<Device> added, List<Device> removed, List<Device> edited) {
    foreach (Device device in removed ) {
      addDevice(device);
    }
    foreach (Device device in added ) {
      removeDevice(device);
    }
    foreach (Device device in edited ) {
      editDevice(device);
    }
  }
  abstract public void addDevice(Device device);
  abstract public void removeDevice(Device device);
  abstract public void editDevice(Device device);
	
}

