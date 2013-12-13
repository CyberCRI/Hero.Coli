using UnityEngine;
using System.Collections;

public abstract class PickableItem : MonoBehaviour {
  protected DNABit _dnaBit;
  public GameObject toDestroy;

  public DNABit getDNABit()
  {
    return _dnaBit;
  }

  protected abstract void addTo();

  public void pickUp()
  {
	ActivePickingDeviceInfoPanel ActivateDeviceInfoPanelScript = gameObject.GetComponent<ActivePickingDeviceInfoPanel>();
	if(ActivateDeviceInfoPanelScript != null){
		ActivateDeviceInfoPanelScript.activate();
	}
	Logger.Log("PickableItem::pickUp", Logger.Level.INFO);
	ActivePickingBioBrickInfoPanel ActivateBioBrickInfoPanelScript = gameObject.GetComponent<ActivePickingBioBrickInfoPanel>();
	if(ActivateBioBrickInfoPanelScript != null){
		ActivateBioBrickInfoPanelScript.activate();
	}
		
    addTo ();
    if(toDestroy)
    {
      Destroy(toDestroy);
    }
    else
    {
      Destroy(gameObject);
    }
  }
}
