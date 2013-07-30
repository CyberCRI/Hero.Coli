using UnityEngine;
using System.Collections;

public class TriggeredRotation : TriggeredBehaviour {
	
	public enum RotationDirection {clockwise, counterClockwise};
	public RotationDirection rotDir = RotationDirection.clockwise;
	public float rotSpeed = 3f;
	public float rotAmount = 0.5f;
	
	public override void triggerStart(){
	}
	
	public override void triggerExit(){
	}
	
	public override void triggerStay(){
		iTween.RotateAdd(gameObject, new Vector3(0, rotSpeed / 10f , 0), 0);
	}
}
