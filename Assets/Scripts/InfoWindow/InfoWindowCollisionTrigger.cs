using UnityEngine;
using System.Collections;

public class InfoWindowCollisionTrigger : InfoWindowTrigger {

  public Collider heroCollider;

  void OnTriggerEnter(Collider other) {
    Debug.Log(this.GetType() + " OnTriggerEnter("+other.ToString()+") _alreadyDisplayed="+_alreadyDisplayed.ToString());
    if(other == heroCollider) {
      displayInfoWindow();
    }
  }

}


