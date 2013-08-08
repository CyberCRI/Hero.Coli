using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CraftFinalizer : MonoBehaviour {
  public Inventory _inventory;
  public CraftZoneManager _craftZoneManager;
         
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

  public void finalizeCraft() {
    //create new device from current biobricks in craft zone
    //TODO names for crafted elements???
    LinkedList<DisplayedBioBrick> dBricks = _craftZoneManager.getCurrentBioBricks();
    LinkedList<BioBrick> bricks = new LinkedList<BioBrick>();
    foreach (DisplayedBioBrick dBrick in dBricks) {
      bricks.AddLast(dBrick._biobrick);
    }
    string randomName = DevicesDisplayer.devicesNames[Random.Range (0, DevicesDisplayer.devicesNames.Count)];
    ExpressionModule craftedModule = new ExpressionModule(randomName+"_module", bricks);
    LinkedList<ExpressionModule> craftedModules = new LinkedList<ExpressionModule>();
    craftedModules.AddLast(craftedModule);
    Device craftedDevice = Device.buildDevice(randomName, craftedModules);
    _inventory.askAddDevice(craftedDevice);
  }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
