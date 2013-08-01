using UnityEngine;
using System;
using System.Collections.Generic;

abstract class DeviceContainer : MonoBehaviour {
	
  protected List<Device> _devices;

  public DevicesDisplayer _displayer;
	
  abstract public void OnChanged(List<Device> removed, List<Device> added, List<Device> edited);
	
}

