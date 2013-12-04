using UnityEngine;
using System.Collections;

public class Life : MonoBehaviour {

	void OnParticleCollision(GameObject obj){
		obj.GetComponent<AmpicillinCollider>();
		if(obj)
			Debug.Log("Losing Life");
	}


}
