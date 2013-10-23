using UnityEngine;
using System.Collections.Generic;

public abstract class DeviceContainer : MonoBehaviour {

  public enum AddingResult {
    SUCCESS,
    FAILURE_SAME_NAME,
    FAILURE_SAME_BRICKS,
    FAILURE_SAME_DEVICE,
    FAILURE_DEFAULT
  }

  public static string _displayerName = "DevicesDisplayersPanel";

  protected List<Device> _devices = new List<Device>();
  public DevicesDisplayer _displayer;
	
  public void UpdateData(List<Device> added, List<Device> removed, List<Device> edited) {
    Logger.Log("DeviceContainer::UpdateData("
      +"added="+Logger.ToString<Device>(added)
      +", removed="+Logger.ToString<Device>(removed)
      +", edited="+Logger.ToString<Device>(edited)
      +")", Logger.Level.DEBUG);
    foreach (Device device in added ) {
      askAddDevice(device);
    }
    Logger.Log("DeviceContainer::UpdateData added done", Logger.Level.TRACE);
    foreach (Device device in removed ) {
      removeDevice(device);
    }
    Logger.Log("DeviceContainer::UpdateData removed done", Logger.Level.TRACE);
    foreach (Device device in edited ) {
      editDevice(device);
    }
    Logger.Log("DeviceContainer::UpdateData edited done", Logger.Level.TRACE);
  }
  
  abstract public AddingResult askAddDevice(Device device);
  abstract public void removeDevice(Device device);
  abstract public void editDevice(Device device);

 // Use this for initialization
 protected void Awake () {
   Logger.Log("DeviceContainer::Awake()", Logger.Level.TRACE);
   _displayer = (DevicesDisplayer)GameObject.Find (_displayerName).GetComponent<DevicesDisplayer>();
 }
}

