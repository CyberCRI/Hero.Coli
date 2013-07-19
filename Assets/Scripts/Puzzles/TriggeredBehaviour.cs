using UnityEngine;
using System.Collections;

public abstract class TriggeredBehaviour : MonoBehaviour {
	public abstract void onTriggerStart();
	public abstract void onTriggerStay();
	public abstract void onTriggerExit();
}
