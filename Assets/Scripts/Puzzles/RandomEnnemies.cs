using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomEnnemies : MonoBehaviour {
	
	public GameObject toInstanciate;
	public int nbOfEnemies = 2;
	public float delayBetweenMov = 0.5f;
	
	// Use this for initialization
	void Awake() {
		for(int i = 0 ; i < nbOfEnemies; i++){
			GameObject enemy = (GameObject) Instantiate(toInstanciate);
			enemy.transform.parent = transform;
			enemy.transform.localPosition = RandomPosition();
			moveRandom(enemy);
		}
	}
	
	private void moveRandom(GameObject obj){
		Vector3 pos = RandomPosition();
		float time = Random.Range(1f, 5f);
		float delay = Random.Range(0f, 1f);
		
		iTween.MoveTo(obj, iTween.Hash(
			"position", pos, 
			"time", time,
			"delay", delay,
			"orienttopath", true,
			"islocal", true,
			"easetype", iTween.EaseType.easeInOutQuad,
			"oncompletetarget", gameObject,
			"oncomplete", "moveRandom",
			"oncompleteparams", obj
		));
	}
			
	private Vector3 RandomPosition(){
		return new Vector3(
			Random.Range (-5f, 5f),
			0,
			Random.Range (-5f, 5f)
		);
	}
}
