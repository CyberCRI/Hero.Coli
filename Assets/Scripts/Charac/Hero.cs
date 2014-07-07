using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Hero : MonoBehaviour {


	public LifeLogoAnimation lifeAnimation;
	public EnergyLogoAnimation energyAnimation;
	Medium _medium;

	//Life

	private float _life = 1f;
  private static float _lifeRegen = 0.1f;
	private Life _lifeManager;

	//Energy.
	private float _energy = 1f;
  private float _maxMediumEnergy = 1f;
	private float _energyBefore = 1f;
	private float _lowEnergyDpt = 3*_lifeRegen;
	

  private bool _pause;
	private bool _isAlive;

	private  object _jointType = typeof(SpringJoint);



	public void Pause(bool pause)
	{
	  _pause = pause;
 	}	

	public bool isPaused()
	{
	  return _pause;
	}


	//getter and setter for energy
	public float getEnergy() {
		return _energy;
	}
	

	public void setEnergy(float energy) {
		if (energy > 1f)
    {
      energy = 1f;
    }
    else if(energy < 0)
    {
			energy = 0;
    }
    _medium.setEnergy(energy*_maxMediumEnergy);
    _energy = energy;
	}


  //energy in ReactionEngine scale (not in percent or ratio)
  public void subEnergy(float energy) {
    _medium.subEnergy(energy);
  }

	public void DisplayEnergyAnimation()  {
		if(energyAnimation.isPlaying == false) {
			 energyAnimation.Play();
		}
	}


	//Getter & setter for the life.
	public float getLife() {
    return _lifeManager.getLife();
	}

	public void setLife(float life) {
    _lifeManager.setLife(life);
  }
    
  public void addLife(float life) {
    _lifeManager.addVariation(life);
  }
    
  public void subLife(float life) {
    _lifeManager.addVariation(-life);
  }

  void Awake ()
  {
    _lifeManager = new Life(_life, _lifeRegen);
  }
  
	void Start (){

    gameObject.SetActive(true);
		_isAlive = true;

    //LinkedList<Medium> mediums = ReactionEngine.get ().getMediumList();
		_medium = ReactionEngine.getMediumFromId(1, ReactionEngine.get ().getMediumList());
    _maxMediumEnergy = _medium.getMaxEnergy();
    _energy = _medium.getEnergy()/_maxMediumEnergy;

	}
  
	void Update() {
    if(!_pause)
    {
      _lifeManager.regen(Time.deltaTime);
      _energy = _medium.getEnergy()/_maxMediumEnergy;

      if (Input.GetKey(KeyCode.R))
      {
		    _lifeManager.addVariation(1f);
      }
      if (Input.GetKey(KeyCode.F)) {
        setEnergy(1f);
      }

      // dammage in case of low energy
      if (_energy <= 0.05f)   _lifeManager.addVariation(- Time.deltaTime * _lowEnergyDpt);


		  // Life animation when life is reducing
		  if (_lifeManager.getVariation() < 0)
      {
        if(lifeAnimation.isPlaying == false){
				  lifeAnimation.Play();
			  }
		  }

		  // Energy animation when energy is reducing
		  if (_energy < _energyBefore) {
		    DisplayEnergyAnimation();
		  }
		  _energyBefore = _energy;


		  _lifeManager.applyVariation();
 		  if(_lifeManager.getLife() == 0f && (_isAlive))
		  {
			  _isAlive = false;
			  StartCoroutine(RespawnCoroutine());
		  }
    }
	}


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
	//Respawn function after death
	IEnumerator RespawnCoroutine() {

	    CellControl cc = GetComponent<CellControl>();
	    cc.enabled = false;
		CellControl _control = transform.GetComponent<CellControl>();
		if(_control.GetBox())
		{
			Destroy(_control.GetBox().transform.GetComponent<SpringJoint>() as SpringJoint);
			_control.SetBox(null);
			_control.SetIsDragging(false);
		}
	//	Destroy(transform.hingeJoint);
		//Logger.Log (""+transform.hingeJoint,Logger.Level.WARN);
	    yield return new WaitForSeconds(2F);

		    cc.enabled = true;
			
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
		
        _isAlive = true;
        cc.reset();
        setLife(1f);
	}	

}