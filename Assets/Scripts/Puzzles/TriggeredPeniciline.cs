using UnityEngine;
using System.Collections;

public class TriggeredPeniciline : TriggeredBehaviour {

	public override void triggerStart(){
		particleSystem.enableEmission = true;
	}
	
	public override void triggerStay(){
		
	}
	
	public override void triggerExit(){
		particleSystem.enableEmission = false;
	}
	
}
