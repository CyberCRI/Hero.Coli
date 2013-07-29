using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Collider))]
public abstract class TriggerBehaviour : MonoBehaviour {
	
	public List<TriggeredBehaviour> toTrigger;

}
