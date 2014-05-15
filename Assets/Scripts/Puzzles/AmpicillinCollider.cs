using UnityEngine;
using System.Collections;

public class AmpicillinCollider : MonoBehaviour {
  //TODO extract to config file
  private static float _dpt = 0.1f;
  
	void OnParticleCollision(GameObject obj){
    Hero hero = obj.GetComponent<Hero>();
		if(hero){      
      hero.subLife(_dpt);
		}
	}
}
