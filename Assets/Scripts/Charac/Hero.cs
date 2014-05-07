using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Hero : MonoBehaviour{
	public LifeLogoAnimation lifeAnimation;
	public EnergyLogoAnimation energyAnimation;
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
  private float _maxMediumEnergy = 1f;
  private float _lowEnergyThreshold = 0.05f;
  private bool _pause;
	private bool _isLiving;

	private float _energyBefore = 1f;

  public void Pause(bool pause)
  {
    _pause = pause;
  }

  public bool isPaused()
  {
    return _pause;
  }

	public float getEnergy() {
		return _energy;
	}

	public void setEnergy(float energy) {
		if (energy > 1f) {energy = 1f;}
		if(energy < 0) 
			energy = 0;
    _medium.setEnergy(energy*_maxMediumEnergy);
    _energy = energy;
	}

  //energy in ReactionEngine scale (not in percent or ratio)
  public void subEnergy(float energy) {



    _medium.subEnergy(energy);
		//_energyBefore = _energy;
  }

	public void DisplayEnergyAnimation()
	{

    if (_energyBefore - _energy > 0)
    {
		  if(energyAnimation.isPlaying == false) {
			  energyAnimation.Play();
		  }
	  }

		/*if (_energy < _energyBefore)
		{
			if(EnergyAnimation.isPlaying == false){
				EnergyAnimation.Play();
			}
		}*/

		_energyBefore = _energy;

		/*float totalEnergy= 0f;
		LinkedList<IReaction> Buffreac = _medium.getReactions();

		foreach(IReaction react in Buffreac)
		{
			totalEnergy += react.getEnergyCost();
		}
		Logger.Log ("energie totale :"+totalEnergy,Logger.Level.INFO);
		if(totalEnergy > _medium.getEnergyProductionRate() )
		{
			if(EnergyAnimation.isPlaying == false){
				EnergyAnimation.Play();
			}
		}*/
	}



	//Getter & setter for the life.
	private float _life = 1f;
	public float getLife() {
		return _life;
	}
	public void setLife(float life) {
		if (life >= 1f)
			life = 1f;
		if (life <= 0f) {
			life = 0f;
		}
		_life = life;
	}

	public void subLife(float life) {
		if(life >0.01){
			if(lifeAnimation.isPlaying == false){
				lifeAnimation.Play();
			}
		}
			
			_life -= life;

	}
  
	void Start (){
		//Click to move variable.
      	//	_destination = mover.position;


    gameObject.SetActive(true);
		_isLiving = true;



    LinkedList<Medium> mediums = ReactionEngine.get ().getMediumList();
    _medium = ReactionEngine.getMediumFromId(1, mediums);
    _maxMediumEnergy = _medium.getMaxEnergy();
    _energy = _medium.getEnergy()/_maxMediumEnergy;

		//FIXME light is undefined
      	//light.enabled = false;



	}
  
	void Update() {
    if(!_pause)
    {
  		setLife(getLife() + Time.deltaTime * _lifeRegen);
      _energy = _medium.getEnergy()/_maxMediumEnergy;
      if(_energy < _lowEnergyThreshold)
      {
        subLife(Time.deltaTime * _lowEnergyDmg);
      }
      if (Input.GetKey(KeyCode.R))
      {
        setLife(1f);
      }
      if (Input.GetKey(KeyCode.F)) {
        setEnergy(1f);
      }
  
      if ((_life <= 0) && (_isLiving))
      {
	    	_life = 0f;
			_isLiving = false;
			StartCoroutine(RespawnCoroutine());
	    }
	    //Logger.Log ("Hero::_medium.getEnergy()="+_medium.getEnergy()+", getEnergy()="+getEnergy(), Logger.Level.ONSCREEN);
    }

		DisplayEnergyAnimation();
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

 	void OnTriggerEnter(Collider collision)
 	{
	    PickableItem item = collision.GetComponent<PickableItem>();
	    if(null != item)
    {
  		Logger.Log("Hero::OnTriggerEnter collided with DNA! bit="+item.getDNABit(), Logger.Level.INFO);
  		item.pickUp();
    }
	
  }


	private bool _spawn01 = false;
	private bool _spawn02 = false;
	private bool _spawn03 = false;
	private bool _spawn04 = false;
	private bool _spawn05 = false;
	private bool _spawn06 = false;
	private bool _spawn07 = false;
	private bool _spawn08 = false;

 	void OnTriggerExit(Collider col) {
 		switch(col.name) {
	      	case "Checkpoint01":
		        _spawn01 = true;
		        break;
	      	case "Checkpoint02":
		        _spawn01 = false;
		        _spawn02 = true;
		        break;
	       	case "Checkpoint03":
	       		_spawn01 = false;
		        _spawn02 = false;
		        _spawn03 = true;
		        break;
		    case "Checkpoint04":
		        _spawn01 = false;
		        _spawn02 = false;
		        _spawn03 = false;
		        _spawn04 = true;
		        break;
		    case "Checkpoint05":
		        _spawn01 = false;
		        _spawn02 = false;
		        _spawn03 = false;
		        _spawn04 = false;
		        _spawn05 = true;
		        break;
		    case "Checkpoint06":
		        _spawn01 = false;
		        _spawn02 = false;
		        _spawn03 = false;
		        _spawn04 = false;
		        _spawn05 = false;
		        _spawn06 = true;
		        break;
		    case "Checkpoint07":
		        _spawn01 = false;
		        _spawn02 = false;
		        _spawn03 = false;
		        _spawn04 = false;
		        _spawn05 = false;
		        _spawn06 = false;
		        _spawn07 = true;
		        break;
		    case "Checkpoint08":
		        _spawn01 = false;
		        _spawn02 = false;
		        _spawn03 = false;
		        _spawn04 = false;
		        _spawn05 = false;
		        _spawn06 = false;
		        _spawn07 = false;
		        _spawn08 = true;
		        break;
	 	}
 	}

	IEnumerator RespawnCoroutine() {

	    CellControl cc = GetComponent<CellControl>();
	    cc.enabled = false;
		
		//GameObject sr = GameObject.Find("StrongRock1");
		//PushableBox pb = sr.GetComponent<PushableBox>();

	    yield return new WaitForSeconds(2F);

		    cc.enabled = true;
		    setLife(1f);
			
			foreach (PushableBox box in FindObjectsOfType(typeof(PushableBox))) {
				box.resetPos();
			}
		    
		    if (_spawn01 == true) {
				GameObject respawn01 = GameObject.Find("Checkpoint01");
				gameObject.transform.position = respawn01.transform.position;
			}
			else if (_spawn02 == true) {
				GameObject respawn02 = GameObject.Find("Checkpoint02");
				gameObject.transform.position = respawn02.transform.position;
			}
			else if (_spawn03 == true) {
				GameObject respawn03 = GameObject.Find("Checkpoint03");
				gameObject.transform.position = respawn03.transform.position;
			}
			else if (_spawn04 == true) {
				GameObject respawn04 = GameObject.Find("Checkpoint04");
				gameObject.transform.position = respawn04.transform.position;
			}
			else if (_spawn05 == true) {
				GameObject respawn05 = GameObject.Find("Checkpoint05");
				gameObject.transform.position = respawn05.transform.position;
			}
			else if (_spawn06 == true) {
				GameObject respawn06 = GameObject.Find("Checkpoint06");
				gameObject.transform.position = respawn06.transform.position;
			}
			else if (_spawn07 == true) {
				GameObject respawn07 = GameObject.Find("Checkpoint07");
				gameObject.transform.position = respawn07.transform.position;
			}
			else if (_spawn08 == true) {
				GameObject respawn08 = GameObject.Find("Checkpoint08");
				gameObject.transform.position = respawn08.transform.position;
			}
		
			_isLiving = true;
	}	

}