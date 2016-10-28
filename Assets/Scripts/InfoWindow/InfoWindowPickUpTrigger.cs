using UnityEngine;
using System.Collections;

public class InfoWindowPickUpTrigger : InfoWindowTrigger, IPickable {

  public void OnPickedUp() {
    Logger.Log("InfoWindowPickUpTrigger::OnPickedUp() _alreadyDisplayed="+_alreadyDisplayed.ToString());
    displayInfoWindow();
  }

}


