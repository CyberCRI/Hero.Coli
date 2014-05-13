using UnityEngine;
using System.Collections;

public class AmpicillinCollider : MonoBehaviour {

	void OnParticleCollision(GameObject obj){
		Hero cellia = obj.GetComponent<Hero>();
		if(cellia){
			cellia.lifeManager.AddVariation(0.1f,false);
			//Debug.Log("HITPlayer");
		}
		BigBadGuy badGuy = obj.GetComponent<BigBadGuy>();	
		if(badGuy){
			//Debug.Log ("HITPeni");
		}
	}
}
