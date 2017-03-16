using System.Collections.Generic;

public static class DeviceBuilder {
	public static Device createDeviceFromData(DeviceData deviceData) {
			var deviceDataBricks = new LinkedList<BioBrick> ();
			foreach (var brick in deviceData.bricks) {
			deviceDataBricks.AddLast(BiobrickBuilder.createBioBrickFromData (brick));
			}
		ExpressionModule deviceModule = new ExpressionModule(deviceData.id, deviceDataBricks);
		LinkedList<ExpressionModule> deviceModules = new LinkedList<ExpressionModule>();
		deviceModules.AddLast(deviceModule);
		return Device.buildDevice(deviceData.id, deviceModules);
	}
}
