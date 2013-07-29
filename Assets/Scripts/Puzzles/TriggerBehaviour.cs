using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public abstract class TriggerBehaviour : MonoBehaviour {
	
	public TriggeredBehaviour toTrigger;
	
	public abstract void onCollisionEnter();
	public abstract void onCollisionStay();
	public abstract void onCollisionExit();

}
