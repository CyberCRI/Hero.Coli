using System;
using System.Collections.Generic;

class Inventory : DeviceContainer
{
	private PromoterBrick 		testPromoter = new PromoterBrick();
	private RBSBrick 			testRBS = new RBSBrick();
	private GeneBrick 			testGene = new GeneBrick();
	private TerminatorBrick 	testTerminator = new TerminatorBrick();
	private List<BioBrick> 		testBricks = new List<BioBrick>();
		
	private Device getTestDevice() {
		
		testBricks.Add(testPromoter);
		testBricks.Add(testRBS);
		testBricks.Add(testGene);
		testBricks.Add(testTerminator);
		
		ExpressionModule testModule = new ExpressionModule("testModule", testBricks); 
		List<ExpressionModule> testModules = new List<ExpressionModule>();
		testModules.Add(testModule);
		
		Device testDevice = new Device("testDevice", testModules);
		
		return testDevice;
	}
	
	public Inventory() {
		//by default: contains a test device
		List<Device> devices = new List<Device>();
		devices.Add(getTestDevice());
		_devices = devices;
		
		OnChanged(new List<Device>(), new List<Device>(), _devices);
	}
	
	public override void OnChanged(List<Device> removed, List<Device> added, List<Device> edited) {
		Device head = added[0];
		_displayer.OnChange(DevicesDisplayer.DeviceType.Inventoried, removed, added, edited);
	}
}

