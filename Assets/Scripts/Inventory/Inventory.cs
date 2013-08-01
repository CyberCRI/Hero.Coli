using System;
using System.Collections.Generic;

class Inventory : DeviceContainer
{
					
	//promoter
	private static float testbeta = 10.0f;
	private static string testformula = "![0.8,2]LacI";
	//rbs
	private static float testrbsFactor = 1.0f;
	//gene
	private static string testproteinName = "testProtein";
	//terminator
	private static float testterminatorFactor = 1.0f;
	
		
	private static Device getTestDevice() {
		
		Device testDevice = Device.buildDevice("testDevice", testbeta, testformula, testrbsFactor, testproteinName, testterminatorFactor);
		
		return testDevice;
	}
	
	public Inventory() {
		//by default: contains a test device
		List<Device> devices = new List<Device>();
		devices.Add(getTestDevice());
		_devices = devices;
		
		OnChanged(new List<Device>(), new List<Device>(), _devices);
	}
	
	//TODO
	public override void OnChanged(List<Device> removed, List<Device> added, List<Device> edited) {
		/*
		Device head = added[0];
		_displayer.OnChange(DevicesDisplayer.DeviceType.Inventoried, removed, added, edited);
		*/
	}
}

