
using System;
using System.Collections.Generic;

class Equipment : DeviceContainer
{
	public Equipment() {
		//by default, nothing's equiped
		_devices = new List<Device>();
	}
	
	public override void OnChanged(List<Device> removed, List<Device> added, List<Device> edited) {
		
	}
}

