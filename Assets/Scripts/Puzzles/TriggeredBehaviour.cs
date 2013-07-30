using UnityEngine;
using System.Collections;

public abstract class TriggeredBehaviour : MonoBehaviour {
	public abstract void triggerStart();
	public abstract void triggerStay();
	public abstract void triggerExit();
}
