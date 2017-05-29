using UnityEngine;
using System.Collections;

public class InfoWindowTrigger : MonoBehaviour {

  public string infoWindowCode;

  protected bool _alreadyDisplayed;

  void Start () {
    _alreadyDisplayed = false;
  }

	protected void displayInfoWindow(bool playSound = true)
  {
    if(!_alreadyDisplayed)
    {
      // Debug.Log(this.GetType() + " call to InfoWindowManager");
      InfoWindowManager.displayInfoWindow(infoWindowCode, playSound);
      _alreadyDisplayed = true;
    }
  }
}


