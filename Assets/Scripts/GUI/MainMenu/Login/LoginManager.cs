using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour {

	public Text feedback;

	public void validate(string inputString) {
		feedback.text = inputString;
		RedMetricsManager.get ().sendEvent(TrackingEvent.LOGIN, new CustomData(CustomDataTag.LOGINID, inputString));
	}
}
