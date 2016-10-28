using UnityEngine;

public class ModalPickUpTrigger : ModalTrigger, IPickable {

  public void OnPickedUp() {
    // Debug.Log(this.GetType() + " OnPickedUp() _alreadyDisplayed="+_alreadyDisplayed.ToString());
    displayModal();
  }
}