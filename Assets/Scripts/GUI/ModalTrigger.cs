using UnityEngine;

public class ModalTrigger : MonoBehaviour
{

	public string modalCode;

	protected bool _alreadyDisplayed;

	void Start ()
	{
		_alreadyDisplayed = false;
	}

	protected void displayModal (bool playSound)
	{
		if (!_alreadyDisplayed) {
			// Debug.Log(this.GetType() + " call to ModalManager");
			ModalManager.setModal (modalCode, true, playSound);
			_alreadyDisplayed = true;
		}
	}
}
