using UnityEngine;

public class TriggeredDoor : TriggeredBehaviour {

	public Transform moveTo;
	public float speed = 10f;
	public float closeDelay = 0f;
	
	private Vector3 origin;
	
	void Start(){
		origin = transform.position;
	}
	
	public override void triggerStart(){
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", moveTo,
			"speed", speed,
			"easetype", iTween.EaseType.easeInOutQuad
		));
	}
	
	public override void triggerExit(){
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", origin,
			"speed", speed,
			"easetype", iTween.EaseType.easeInOutQuad,
			"delay", closeDelay
		));
	}
	
	public override void triggerStay(){}
}
