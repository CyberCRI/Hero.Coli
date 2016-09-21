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

  //TODO DeviceDisplayer
  public static string _displayerName = "DevicesDisplayersPanel";

  protected List<Device> _devices = new List<Device>();
  protected static DevicesDisplayer _displayer;

  public bool contains(Device device)
  {
    return _devices.Exists(d => d.Equals(device));
  }

  public bool exists(System.Predicate<Device> predicate)
  {
    return _devices.Exists(predicate);
  }

  public void removeAll(System.Predicate<Device> predicate)
  {
    List<Device> devicesToRemove = new List<Device>();
    foreach(Device device in _devices)
    {
        if(predicate(device))
        {
          devicesToRemove.Add(device);
        }
    }
    removeDevices(devicesToRemove);
  }
	
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
    removeDevices(removed);
    Logger.Log("DeviceContainer::UpdateData removed done", Logger.Level.TRACE);
    foreach (Device device in edited ) {
      editDevice(device);
    }
    Logger.Log("DeviceContainer::UpdateData edited done", Logger.Level.TRACE);
  }
  
  abstract public AddingResult askAddDevice(Device device, bool reportToRedMetrics = false);
  abstract public void removeDevice(Device device);
  abstract public void removeDevices(List<Device> devices);
  abstract public void editDevice(Device device);

 // Use this for initialization
 protected virtual void Start () {
   Logger.Log("DeviceContainer::Start()", Logger.Level.TRACE);
   _displayer = safeGetDisplayer();
 }

  protected DevicesDisplayer safeGetDisplayer()
  {
    if(_displayer == null)
    {
      _displayer = DevicesDisplayer.get();
    }
    return _displayer;
  }
}

