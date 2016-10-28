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
      // Debug.Log(this.GetType() + " call to InfoWindowManager");
      InfoWindowManager.displayInfoWindow(infoWindowCode);
      _alreadyDisplayed = true;
    }
  }
}


