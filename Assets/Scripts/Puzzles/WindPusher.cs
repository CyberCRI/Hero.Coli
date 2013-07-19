using UnityEngine;
using System.Collections;

public class WindPusher : MonoBehaviour {

	public float force = 20;
	
    void OnTriggerStay(Collider other) {
		if (other.attachedRigidbody){
			Vector3 push = this.transform.rotation * new Vector3(0,0,1);
        	other.attachedRigidbody.AddForce(push * force);
		}
	}
}
