using UnityEngine;
using System.Collections;

public class Life : MonoBehaviour {

	void OnParticleCollision(GameObject obj){
		obj.GetComponent<AmpicilineCollider>();
		if(obj)
			Debug.Log("Loosing Life");
	}
}
