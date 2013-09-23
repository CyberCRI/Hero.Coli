using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PotionsDisplayer : MonoBehaviour {
	
	private List<Potion> _potions = new List<Potion>();
	public GameObject _potionPrefab;
	
	private float _timeAtLastFrame = 0f;
    private float _timeAtCurrentFrame = 0f;
    private float _deltaTime = 0f;	
	private float _deltaTimeThreshold = 0.2f;
	
	//TODO use real potion width
	static private float _width = 50.0f;
	static private Vector3 _positionOffset = new Vector3(-147.0f, 23.0f, -0.1f);
	
	
	public void addPotion(int potionID) {
		Debug.Log("addPotion("+potionID+")");
		if(!_potions.Exists(potion => potion.getID() == potionID)) { 
			Vector3 localPosition = _positionOffset + new Vector3(_potions.Count*_width, 0.0f, 0.0f);
			Potion potion = Potion.Create(gameObject.transform, localPosition, potionID);
			_potions.Add(potion);
		}
	}
	
	public void removePotion(int potionID) {
		Debug.Log("removePotion("+potionID+")");
		Potion toRemove = _potions.Find(potion => potion.getID() == potionID);
		if(toRemove != null) {
			_potions.Remove(toRemove);
			toRemove.Remove();
			for(int i = 0; i < _potions.Count; i++) {
				Vector3 newLocalPosition = _positionOffset + new Vector3(i*_width, 0.0f, 0.0f);
				_potions[i].Redraw(newLocalPosition);
			}
		}
	}
	
	// Use this for initialization
	void Start () {		
		/*
		for(int i = 0; i < 5; i++) {
			addPotion (i);
		}
		*/
	}
	
	// Update is called once per frame
	void Update () {
		
		_timeAtCurrentFrame = Time.realtimeSinceStartup;
        _deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
		
		if(_deltaTime > _deltaTimeThreshold) {
			if (Input.GetKey(KeyCode.C)) {//CREATE
				int randomID = Random.Range(0, 12000);
	        	addPotion(randomID);
			}
	        if (Input.GetKey(KeyCode.R)) {//REMOVE
				if(_potions.Count > 0) {
					int randomIdx = Random.Range(0, _potions.Count);
					Potion randomPotion = _potions[randomIdx];
		        	removePotion(randomPotion.getID());
				}
			}
			_timeAtLastFrame = _timeAtCurrentFrame;
		}
	}
}
