using UnityEngine;

public class CollisionTrigger : TriggerBehaviour {
	
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
