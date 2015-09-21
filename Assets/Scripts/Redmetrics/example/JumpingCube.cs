using UnityEngine;
using System.Collections;

public class JumpingCube : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Space)) {

			CustomData customData = new CustomData();
			customData.Add("velocity", this.gameObject.GetComponent<Rigidbody>().velocity.ToString());

			RedMetricsManager.get ().sendEvent(TrackingEvent.JUMP,
			                                   this.gameObject.transform.position,
			                                   customData,
			                                   Application.loadedLevelName);

			giveImpulsion();
		}

		if(this.gameObject.transform.localPosition.y < -2.0f) {

			CustomData customData = new CustomData();
			customData.Add("velocity", this.gameObject.GetComponent<Rigidbody>().velocity.ToString());

			RedMetricsManager.get ().sendEvent(TrackingEvent.BOUNCE, 
			                                   this.gameObject.transform.position,
			                                   customData,
			                                   Application.loadedLevelName);
			giveImpulsion();
		}
	}

	void giveImpulsion() {
			this.GetComponent<Rigidbody>().velocity = Vector3.zero;
			this.GetComponent<Rigidbody>().AddForce(new Vector3(0, 500.0f, 0));
	}
}
