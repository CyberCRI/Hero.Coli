﻿#define TUTORIAL2

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
#if TUTORIAL2
			RedMetricsManager.get().sendEvent(TrackingEvent.HINT, new CustomData(CustomDataTag.MESSAGE, modalCode));
#endif
			_alreadyDisplayed = true;
		}
	}
}
