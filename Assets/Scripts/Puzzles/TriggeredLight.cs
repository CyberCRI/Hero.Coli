using UnityEngine;
using System.Collections;

public class TriggeredLight : TriggeredBehaviour {
	
	public float maxIntensity =3f;
	public float tweenSpeed = 4f;

	private bool _isStarting = false;
	private bool _isPlaying = false;


	public bool getIsStarting () {return _isStarting;}

	public void setIsPlaying (bool b) { _isPlaying = b;}
	public bool getIsPlaying () {return _isPlaying;}
	
	public override void triggerStart(){
		iTween.Stop(gameObject);
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", GetComponent<Light>().intensity,
			"to", maxIntensity,
			"speed", tweenSpeed,
			"easetype", iTween.EaseType.easeInQuint,
			"onupdate", "updateLight",
			"onupdatetarget",gameObject,
			"oncomplete", "setIsPlaying",
			"oncompletetarget", gameObject,
			"oncompleteparams", true
		));
		_isStarting = true;
	}
	
	private void updateLight(float val){
		GetComponent<Light>().intensity = val;
	}
	
	public override void triggerExit(){
		iTween.Stop(gameObject);
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", GetComponent<Light>().intensity,
			"to", 0,
			"speed", tweenSpeed,
			"easetype", iTween.EaseType.easeInOutQuad,
			"onupdate", "updateLight",
			"onupdatetarget",gameObject,
			"oncomplete", "setIsPlaying",
			"oncompletetarget", gameObject,
			"oncompleteparams", false
		));
		_isStarting = false;
	}
	
	public override void triggerStay(){}

	private void updateLightIntensity()
	{
		float distance = Vector3.Magnitude (transform.parent.position - GameObject.Find("Perso").transform.position);

		//intensity increase when the player get near :: max 9 * maxIntensity
		GetComponent<Light>().intensity = maxIntensity * (1f + 8f*(GetComponent<Light>().range - distance)/GetComponent<Light>().range);

	}

	public void Update() {

		if(_isPlaying)
		{
			updateLightIntensity();
		}

	}

	
}
