using UnityEngine;
using System.Collections;

public class CollisionTrigger : TriggerBehaviour {
	
	/*
	void OnCollisionEnter(Collision collision){
		foreach(TriggeredBehaviour tb in toTrigger)
			tb.triggerStart();
	}
	
	void OnCollisionStay(Collision collision){
		foreach(TriggeredBehaviour tb in toTrigger)
			tb.triggerStay();
	}
	
	void OnCollisionExit(Collision collision){
		foreach(TriggeredBehaviour tb in toTrigger)
			tb.triggerExit();
	}
	*/
	
	void OnTriggerEnter(Collider collision){
		foreach(TriggeredBehaviour tb in toTrigger)
		{
			if(tb.gameObject != null)
				tb.triggerStart();
		}
	}
	
	void OnTriggerStay(Collider collision){
		foreach(TriggeredBehaviour tb in toTrigger)
		{
			if(tb.gameObject != null)
				tb.triggerStay();
		}
	}
	
	void OnTriggerExit(Collider collision){
		foreach(TriggeredBehaviour tb in toTrigger)
			{
				if(tb.gameObject != null)
					tb.triggerExit();
			}
	}
}
