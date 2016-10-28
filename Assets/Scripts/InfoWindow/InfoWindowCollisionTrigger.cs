using UnityEngine;
using System.Collections;

public class InfoWindowCollisionTrigger : InfoWindowTrigger {

  public Collider heroCollider;

  void OnTriggerEnter(Collider other) {
    Logger.Log("InfoWindowCollisionTrigger::OnTriggerEnter("+other.ToString()+") _alreadyDisplayed="+_alreadyDisplayed.ToString());
    if(other == heroCollider) {
      displayInfoWindow();
    }
  }

}


