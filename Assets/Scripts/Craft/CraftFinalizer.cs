using UnityEngine;
using System.Collections;

public class CraftFinalizer : MonoBehaviour {
  public Inventory _inventory;
         
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
    _inventory.addDevice(getTestDevice());
  }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
