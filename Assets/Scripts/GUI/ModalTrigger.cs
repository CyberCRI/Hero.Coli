using UnityEngine;

public class ModalTrigger : MonoBehaviour {

  public string modalCode;

  protected bool _alreadyDisplayed;

  void Start () {
    _alreadyDisplayed = false;
  }

  protected void displayModal()
  {
    if(!_alreadyDisplayed)
    {
      Logger.Log("call to ModalManager", Logger.Level.TRACE);
      ModalManager.setModal(modalCode);
      _alreadyDisplayed = true;
    }
  }
}
