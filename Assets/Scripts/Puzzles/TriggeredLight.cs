using UnityEngine;
using System.Collections;

public class TriggeredLight : TriggeredBehaviour {
	
	public float maxIntensity = 2f;
	public float tweenSpeed = 2f;
	
	public override void triggerStart(){
		iTween.Stop(gameObject);
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", light.intensity,
			"to", maxIntensity,
			"speed", tweenSpeed,
			"easetype", iTween.EaseType.easeInOutQuad,
			"onupdate", "updateLight"
		));
	}
	
	private void updateLight(float val){
		light.intensity = val;
	}
	
	public override void triggerExit(){
		iTween.Stop(gameObject);
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", light.intensity,
			"to", 0,
			"speed", tweenSpeed,
			"easetype", iTween.EaseType.easeInOutQuad,
			"onupdate", "updateLight"
		));
	}
	
	public override void triggerStay(){}
	
}
