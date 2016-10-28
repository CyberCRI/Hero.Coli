using UnityEngine;

public class ModalPickUpTrigger : ModalTrigger, IPickable {

  public void OnPickedUp() {
    Debug.Log(this.GetType() + " ModalPickUpTrigger::OnPickedUp() _alreadyDisplayed="+_alreadyDisplayed.ToString());
    displayModal();
  }
}