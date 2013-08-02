using UnityEngine;
using System.Collections;

public class BigBadGuy : MonoBehaviour {
	
	public int life = 50;
	public float shrinkSpeed = 3;
	
	private float step;
	
	void Start(){
		step = transform.localScale.x / life;
	}
	
	void OnParticleCollision(GameObject obj){
		obj.GetComponent<AmpicilineCollider>();
		if(obj){
			
			Vector3 newScale = transform.localScale - new Vector3(step, step, step);
			transform.localScale = newScale;
			life--;
			
			if(life == 0)
				Destroy(gameObject);
				
		}
	}
}
