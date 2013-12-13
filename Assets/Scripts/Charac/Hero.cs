using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Hero : MonoBehaviour{

	Medium _medium;
	
	/*
	//Getter & setter for the move speed.
	private float _moveSpeed = 3f;
	public float getMoveSpeed() {
		return _moveSpeed;
	}
	public void setMoveSpeed(float moveSpeed) {
		if (moveSpeed < 0)
			moveSpeed = 0; 
			_moveSpeed = moveSpeed;
	}
	*/
	
	//References for the click to move controler.
	//	private float _moveSmooth = .5f;
	//	private Vector3 _destination = Vector3.zero;

	/*
	//Getter & setter for the inventory.
	private int _collected;
	public int getCollected() {
		return _collected;
	}
	public void setCollected(int collected) {
		_collected = collected;
	}*/

  //Life
  private static float _lifeRegen = 0.1f;
  //private float _lowEnergyDmg = 0.15f;
  private static float _lowEnergyDmg = 3*_lifeRegen;

	//Getter & setter for the energy.
	private float _energy = 1f;
  private float _maxEnergy = 1f;
  private float _lowEnergyThreshold = 0.05f;

	public float getEnergy() {
		return _energy;
	}
	public void setEnergy(float energy) {
		if (energy > 1f) {energy = 1f;}
		if(energy < 0) 
			energy = 0; 
			_energy = energy;
	}
  public void subEnergy(float energy) {
    _medium.subEnergy(energy);
  }

	//Getter & setter for the life.
	private float _life = 1f;
	public float getLife() {
		return _life;
	}
	public void setLife(float life) {
		if (life > 1f)
			life = 1;
		if (life < 0) {
			life = 0;
			//gameObject.SetActive(false);
			Debug.Log("You died");
			reSpawn();
		}
		_life = life;
	}

	public void subLife(float life) {
		_life -= life;
	}
  
	void Start (){
		//Click to move variable.
      	//	_destination = mover.position;
      	gameObject.SetActive(true);

	    LinkedList<Medium> mediums = ReactionEngine.get ().getMediumList();
	    _medium = ReactionEngine.getMediumFromId(1, mediums);
	    _maxEnergy = _medium.getMaxEnergy();
    _energy = _medium.getEnergy()/_maxEnergy;

		//FIXME light is undefined
      	//light.enabled = false;
	}
  
	void Update() {
		setLife(getLife() + Time.deltaTime * _lifeRegen);
    _energy = _medium.getEnergy()/_maxEnergy;
    if(_energy < _lowEnergyThreshold)
    {
      subLife(Time.deltaTime * _lowEnergyDmg);
    }
    if (Input.GetKey(KeyCode.A))
    {
      setLife(1f);
    }
    //Logger.Log ("Hero::_medium.getEnergy()="+_medium.getEnergy()+", getEnergy()="+getEnergy(), Logger.Level.ONSCREEN);
	}

	/*
	//When the player collects a biobrick.
	public void Collect() {
		setCollected(getCollected() + 1);
	}

	//When the player collects glucose.
	public void winEnergy() {
		setEnergy(getEnergy() + .2f);
	}

	//When the player equiped the improved motility device.
	public void equipImpMoti() {
		setMoveSpeed(getMoveSpeed() + 3f);
	}

	//When the player reacts to the light and emits colors.
	public void emitLight(bool boolean) {
		//FIXME light is undefined
		//light.enabled = boolean;
	}

	public void changeColor(Color color) {
		//FIXME light is undefined
		//light.color = color;
	}
	*/


	private bool _spawnOne = false;
	private bool _spawnTwo = false;

 	void OnTriggerExit(Collider col) {
 		switch(col.name)
    {
      case "CheckpointOne":
        _spawnOne = true;
        break;
      case "CheckpointTwo":
        _spawnOne = false;
        _spawnTwo = true;
        break;
	 	}
 	}

  void OnTriggerEnter(Collider collision){
    PickableItem item = collision.GetComponent<PickableItem>();
    if(null != item)
    {
      Logger.Log("Hero::OnTriggerEnter collided with DNA! bit="+item.getDNABit(), Logger.Level.INFO);
      item.pickUp();
    }
  }

	public void reSpawn() {
		if (_spawnOne == true) {
			GameObject respawnPointOne = GameObject.Find("SpawnOne");
			gameObject.transform.position = respawnPointOne.transform.position;
			setLife(1f);
		}
		else if (_spawnTwo == true) {
			GameObject respawnPointTwo = GameObject.Find("SpawnTwo");
			gameObject.transform.position = respawnPointTwo.transform.position;
			setLife(1f);
		}
	}

}