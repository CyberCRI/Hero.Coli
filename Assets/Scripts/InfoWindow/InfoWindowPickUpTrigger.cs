using UnityEngine;
using System.Collections;

public class InfoWindowPickUpTrigger : InfoWindowTrigger, IPickable {

  public void OnPickedUp() {
    Debug.Log(this.GetType() + " OnPickedUp() _alreadyDisplayed="+_alreadyDisplayed.ToString());
    displayInfoWindow();
  }

}


