using UnityEngine;
using System.Collections;

public class InfoWindowTrigger : MonoBehaviour {

  public string infoWindowCode;

  protected bool _alreadyDisplayed;

  void Start () {
    _alreadyDisplayed = false;
  }

  protected void displayInfoWindow()
  {
    if(!_alreadyDisplayed)
    {
      Logger.Log("call to InfoWindowManager", Logger.Level.TEMP);
      InfoWindowManager.displayInfoWindow(infoWindowCode);
      _alreadyDisplayed = true;
    }
  }
}


