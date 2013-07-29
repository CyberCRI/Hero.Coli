using UnityEngine;
using System.Collections;

public class CollisionTrigger : TriggerBehaviour {
	
	void OnTriggerEnter(Collider collision){
		foreach(TriggeredBehaviour tb in toTrigger)
			tb.triggerStart();
	}
	
	void OnTriggerStay(Collider collision){
		foreach(TriggeredBehaviour tb in toTrigger)
			tb.triggerStay();
	}
	
	void OnTriggerExit(Collider collision){
		foreach(TriggeredBehaviour tb in toTrigger)
			tb.triggerExit();
	}
}
