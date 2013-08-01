using UnityEngine;
using System.Collections;

public class TriggeredPathEnnemy : TriggeredBehaviour {
	
	public string iEventName = "PathEnnemy";
	
	private bool activated = false;
	
	public override void triggerStart(){
		if(!activated){
			iTweenEvent.GetEvent(gameObject, iEventName).Play();
			activated = true;	
		}
	}
	
	public override void triggerExit(){
	}
	
	public override void triggerStay(){
	}
}
