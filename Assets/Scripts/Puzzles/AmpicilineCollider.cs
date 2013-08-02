using UnityEngine;
using System.Collections;

public class AmpicilineCollider : MonoBehaviour {

	void OnParticleCollision(GameObject obj){
		if(obj.GetComponent<CellControl>()){
			//Debug.Log("HITPlayer");
		}
			
		if(obj.GetComponent<BigBadGuy>()){
			//Debug.Log ("HITPeni");
		}
	}
}
