using UnityEngine;
using System.Collections;

public class BigBadGuy : MonoBehaviour {
	
	public int life = 50;
	public float shrinkSpeed = 3;
	public Hero hero;
	
	private float step;
	
	void Start(){
		step = transform.localScale.x / life;
	}
	
	void OnParticleCollision(GameObject obj){
		obj.GetComponent<AmpicillinCollider>();
		if(obj){
			
			Vector3 newScale = transform.localScale - new Vector3(step, step, step);
			transform.localScale = newScale;
			life--;
			
			if(life == 0)
				Destroy(gameObject);
				
		}
	}

	void OnCollisionEnter(Collision col) {
		if (col.collider){
			Hero hero = col.gameObject.GetComponent<Hero>();
			hero.lifeManager.AddVariation(0.5f,false);
		}
	}
	
}
