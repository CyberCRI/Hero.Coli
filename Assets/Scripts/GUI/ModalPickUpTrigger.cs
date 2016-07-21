
public class ModalPickUpTrigger : ModalTrigger, IPickable {

  public void OnPickedUp() {
    Logger.Log("ModalPickUpTrigger::OnPickedUp() _alreadyDisplayed="+_alreadyDisplayed.ToString(), Logger.Level.DEBUG);
    displayModal();
  }
}