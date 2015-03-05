using UnityEngine;
using System.Collections;

public class TriggeredPeniciline : TriggeredBehaviour {

	public override void triggerStart(){
		GetComponent<ParticleSystem>().enableEmission = true;
	}
	
	public override void triggerStay(){
		
	}
	
	public override void triggerExit(){
		GetComponent<ParticleSystem>().enableEmission = false;
	}
	
}
