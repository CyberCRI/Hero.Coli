using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

  //respawn
  private GameObject _lastCheckpoint = null;
  private GameObject _lastNewCell = null;

  static float _disappearingTimeS = 2.0f;
  static float _respawnTimeS = 3.0f;
  static float _popEffectTimeS = 1.0f;
  static private float _baseScale = 145.4339f;
  static private Vector3 _baseScaleVector = new Vector3(_baseScale, _baseScale, _baseScale);
  static private Vector3 _reducedScaleVector = 0.7f*_baseScaleVector;

  private Hashtable _optionsIn = iTween.Hash(
      "scale", _baseScaleVector,
      "time", 0.8f,
      "easetype", iTween.EaseType.easeOutElastic
      );

  private Hashtable _optionsOut = iTween.Hash(
      "scale", _reducedScaleVector,
      "time",_disappearingTimeS,
      "easetype", iTween.EaseType.easeInQuint
      );
  private Hashtable _optionsInAlpha = iTween.Hash(
      "alpha", 1.0f,
      "time", 0.8f,
      "easetype", iTween.EaseType.easeOutElastic
      );

  private Hashtable _optionsOutAlpha = iTween.Hash(
      "alpha", 0.0f,
      "time",_disappearingTimeS,
      "easetype", iTween.EaseType.easeInQuint
      );

	public Life getLifeManager () {return _lifeManager;}

	public void Pause(bool pause)
	{
	  _pause = pause;
 	}	

	public bool isPaused()
	{
	  return _pause;
	}


	public Medium getMedium() { return _medium;}
	public bool isAlive() { return _isAlive;}


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

      if (GameStateController.isShortcutKey("KEY.LIFE"))
      {
		    _lifeManager.addVariation(1f);
      }
      if (GameStateController.isShortcutKey("KEY.ENERGY"))
      {
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

  void setCurrentRespawnPoint(Collider col)
  {
      if(null != col.gameObject.GetComponent<RespawnPoint>()
           && (null == _lastCheckpoint
            || _lastCheckpoint.name != col.gameObject.name))
      {
          _lastCheckpoint = col.gameObject;
          duplicateCell();
      }
  }

  //TODO divide by 2 the chemicals
  void duplicateCell()
  {
      if(null != _lastNewCell)
      {
        Destroy (_lastNewCell);
      }

      _lastNewCell = (GameObject)Instantiate(this.gameObject);

      SavedCell savedCell = (SavedCell)_lastNewCell.AddComponent<SavedCell>();
      savedCell.initialize(this, _lastCheckpoint.transform.position);

      StartCoroutine(popEffectCoroutine(savedCell));
  }

 	void OnTriggerEnter(Collider collision)
 	{
	  PickableItem item = collision.GetComponent<PickableItem>();
	  if(null != item)
    {
  	  Logger.Log("Hero::OnTriggerEnter collided with DNA! bit="+item.getDNABit(), Logger.Level.INFO);
      item.pickUp();
    }
    else
    {
      setCurrentRespawnPoint(collision);
    }
  }

 	void OnTriggerExit(Collider col) {
    setCurrentRespawnPoint(col);
  }

	//Respawn function after death
	IEnumerator RespawnCoroutine() {

        CellControl cc = GetComponent<CellControl>();

        //1. death effect
        deathEffect(cc);
                                    
        yield return new WaitForSeconds(_respawnTimeS);
    
        //1. respawn effect
        respawnCoroutine(cc);
        
    }	
    
    void deathEffect(CellControl cc)
    {
        cc.enabled = false;
        
        iTween.ScaleTo(gameObject, _optionsOut);
        iTween.FadeTo(gameObject, _optionsOutAlpha);        
    }
    
    void respawnCoroutine(CellControl cc)
    {
        iTween.ScaleTo(gameObject, _optionsIn);
        iTween.FadeTo(gameObject, _optionsInAlpha);
        
        cc.enabled = true;      
        foreach (PushableBox box in FindObjectsOfType(typeof(PushableBox))) {
            box.resetPos();
        }

        MineManager.isReseting = true;


        SavedCell savedCell = null;
        if(null != _lastNewCell)
        {
            savedCell = (SavedCell)_lastNewCell.GetComponent<SavedCell>();
            savedCell.resetCollisionState();
            gameObject.transform.position = _lastNewCell.transform.position;
            gameObject.transform.rotation = _lastNewCell.transform.rotation;
        }
        
        _isAlive = true;
        cc.reset();
        setLife(1f);

        StartCoroutine(popEffectCoroutine(savedCell));
    }
    
    IEnumerator popEffectCoroutine(SavedCell savedCell)
    {
        yield return new WaitForSeconds(_popEffectTimeS);
        savedCell.setCollidable(true);
    }
}