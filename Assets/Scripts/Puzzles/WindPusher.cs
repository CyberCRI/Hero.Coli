using UnityEngine;
using System.Collections;

public class WindPusher : MonoBehaviour {

	public float force = 20;
	
	private float length;
	
	void Start(){
		length = transform.localScale.z;
	}
	
	void OnTriggerStart(Collider col){
		/*
		PushableBox pb = col.gameObject.GetComponent<PushableBox>();
		if(pb){
			this.transform.localScale = pb.	
		}*/
	}
	
    void OnTriggerStay(Collider col) {
		if (col.attachedRigidbody){
			Vector3 push = this.transform.rotation * new Vector3(0,0,1);
        	col.attachedRigidbody.AddForce(push * force);
		}
	}
}
