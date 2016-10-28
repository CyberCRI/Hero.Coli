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
      Debug.Log(this.GetType() + " call to ModalManager");
      ModalManager.setModal(modalCode);
      _alreadyDisplayed = true;
    }
  }
}
