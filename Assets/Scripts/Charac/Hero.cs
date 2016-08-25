using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Hero : MonoBehaviour {
    
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public static string gameObjectName = "Perso";
    private static Hero _instance;
    private float _originOffsetY;
    [SerializeField]
    private AmbientLighting _ambientLighting;
    [SerializeField]
    private GameObject[] _childrenToDestroy;
    
    public void destroyChildren()
    {
        foreach (GameObject child in _childrenToDestroy)
        {
            Destroy(child);
        }
    }

    public static Hero get() {
        if (_instance == null)
        {
            GameObject go = GameObject.Find(gameObjectName);
            if(null != go)
            {
                _instance = go.GetComponent<Hero>();
                if(null != _instance) {
                    _instance.initializeIfNecessary();
                }
            }
            else
            {
                Logger.Log("Hero::get couldn't find game object", Logger.Level.ERROR);
            }
        }
        return _instance;
    }
    void Awake()
    {
        Logger.Log("Hero::Awake", Logger.Level.INFO);
        Hero.get();
        _lifeManager = new Life(_life, _lifeRegen);
        this.gameObject.AddComponent<MovementHint>();
    }

    void Start()
    {
        _originOffsetY = 41.11548f;
        _ambientLighting = this.GetComponent<AmbientLighting>();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

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
	
    //Status
    private bool _pause;
	private bool _isAlive;
    private bool _isInjured;
    private bool _isRegen;
    private float _previousLife;

    //respawn
    private GameObject _lastCheckpoint = null;
    private GameObject _lastNewCell = null;


    private static float _respawnTimeS = 1.5f;
    private static float _disappearingTimeSRatio = 0.9f;
    private static float _disappearingTimeS = _disappearingTimeSRatio*_respawnTimeS;
    private float _popEffectTimeS = 1.0f;
    private static float _baseScale = 145.4339f;
    private static Vector3 _baseScaleVector = new Vector3(_baseScale, _baseScale, _baseScale);
    private static Vector3 _reducedScaleVector = 0.7f*_baseScaleVector;
    private List<GameObject> _flagella = new List<GameObject>();
    private static string checkpointSeparator = ".";
    private const string _keyLife = "KEY.LIFE";
    private const string _keyEnergy = "KEY.ENERGY";
        
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
        //"includechildren", false
      );

  private Hashtable _optionsOutAlpha = iTween.Hash(
      "alpha", 0.0f,
      "time",_disappearingTimeS,
      "easetype", iTween.EaseType.easeInQuint
      );

    private void initializeIfNecessary()
    {
        //???
        //gameObject.SetActive(true);
        _isAlive = true;
        
        //LinkedList<Medium> mediums = ReactionEngine.get ().getMediumList();
        _medium = ReactionEngine.getMediumFromId(1, ReactionEngine.get ().getMediumList());
        _maxMediumEnergy = _medium.getMaxEnergy();
        _energy = _medium.getEnergy()/_maxMediumEnergy;
    }

	public Life getLifeManager () {return _lifeManager;}
    
    public void kill() {
        _lifeManager.setSuddenDeath(true);
    }

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
  
	void Update() {
    if(!_pause)
    {
      _lifeManager.regen(Time.deltaTime);
      _energy = _medium.getEnergy()/_maxMediumEnergy;

      if (GameStateController.isShortcutKey(_keyLife, true))
      {
		    _lifeManager.addVariation(1f);
      }
      if (GameStateController.isShortcutKey(_keyEnergy, true))
      {
        setEnergy(1f);
      }

      // dammage in case of low energy
      if (_energy <= 0.05f) {
                _lifeManager.addVariation(- Time.deltaTime * _lowEnergyDpt);
            }


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
                _ambientLighting.Dead();
        
        RedMetricsManager.get ().sendEvent(TrackingEvent.DEATH, null, getLastCheckpointName());
			  StartCoroutine(RespawnCoroutine());
		  }
            float life = _lifeManager.getLife();
          if (_lifeManager.getLife()<= 0.95f)
            {
                if (_isInjured == false)
                {
                    _ambientLighting.SaveCurrentLighting();
                    _isInjured = true;
                }
                if (life >= _previousLife)
                {
                    _isRegen = true;
                }
                else
                {
                    _isRegen = false;
                }
                _ambientLighting.Injured(life);
            }
          else if (_lifeManager.getLife() > 0.95f)
            {
                _isInjured = false;
                _ambientLighting.ResetLighting();
            }
    }

        _previousLife = _lifeManager.getLife();

	}

    public string getLastCheckpointName() {
        string levelName = MemoryManager.get().configuration.getGameMapName();
        string lastCheckpointName = null==_lastCheckpoint?"":_lastCheckpoint.name;
        string result = string.IsNullOrEmpty(lastCheckpointName)?lastCheckpointName:levelName+checkpointSeparator+lastCheckpointName;
        return result;
    }
    
    void manageCheckpoint(Collider col)
    {
        // Respawn
        setCurrentRespawnPoint(col);
        
        // Particle emitters:
        //  inactivate previous (-2)
        //  activate future (+2)
        updateMapElements(0);
    }
    
    void managePickUp(Collider col)
    {
        PickableItem item = col.GetComponent<PickableItem>();
        if(null != item)
        {
            Logger.Log("Hero::managePickUp collided with DNA! bit="+item.getDNABit(), Logger.Level.INFO);
            item.pickUp();
            //RedMetricsManager.get ().sendEvent(TrackingEvent.PICKUP, new CustomData(CustomDataTag.DNABIT, item.getDNABit().getInternalName()));
        }
    }
    
    // Manages sector when entering into the sector.
    void manageSector(Collider col)
    {
        Sector sector = col.GetComponent<Sector>();
        if(null != sector)
        {
            Logger.Log("Hero::manageSector collided with sector="+sector.ToString(), Logger.Level.INFO);
            sector.activate();
        }
    }
    
    void updateMapElements(int checkpointIndex)
    {}

  void setCurrentRespawnPoint(Collider col)
  {
      if(null != col.gameObject.GetComponent<RespawnPoint>()
           && (null == _lastCheckpoint
            || _lastCheckpoint.name != col.gameObject.name))
      {
          _lastCheckpoint = col.gameObject;
          duplicateCell();

          //RedMetrics reporting
          //TODO put equiped devices in customData of sendEvent
          RedMetricsManager.get ().sendEvent(TrackingEvent.REACH);
      }
  }

  //TODO divide by 2 the chemicals
  void duplicateCell()
  {
      if(null != _lastNewCell)
      {
        Destroy (_lastNewCell);
      }
      else
      {
        ModalManager.setModal("FirstCheckpoint");
      }

      _lastNewCell = (GameObject)Instantiate(this.gameObject);

      SavedCell savedCell = (SavedCell)_lastNewCell.AddComponent<SavedCell>();
      savedCell.initialize(this, _lastCheckpoint.transform.position);

      StartCoroutine(popEffectCoroutine(savedCell));
  }

    void OnTriggerEnter(Collider collision)
 	{
        managePickUp(collision);
        manageCheckpoint(collision);
        manageSector(collision);
    }

 	void OnTriggerExit(Collider col) {
        manageCheckpoint(col);
    }

	//Respawn function after death
	IEnumerator RespawnCoroutine() {

        CellControl cc = GetComponent<CellControl>();
        GUITransitioner.get().mainBoundCamera.offset.y = _originOffsetY;

        yield return StartCoroutine(deathEffectCoroutine(cc));        
    }	

    public static void safeFadeTo(GameObject toFade, Hashtable fadeOptions) {
        //TODO find most robust method
        //GameObject body = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(). .transform.FindChild("body_perso");
        //GameObject body = gameObject.transform.FindChild("body_perso").gameObject;
        GameObject body = toFade.transform.FindChild("body_perso").transform.FindChild("body_perso").gameObject;
        if(null != body)
        {
            iTween.FadeTo(body, fadeOptions);
        }
        GameObject dna = toFade.transform.FindChild("dna_perso").transform.FindChild("body_perso").gameObject;
        if(null != dna)
        {
            iTween.FadeTo(dna, fadeOptions);
        }
    }

    private void safeFadeTo(Hashtable hash) {
        safeFadeTo(gameObject, hash);
    }
    
    IEnumerator deathEffectCoroutine(CellControl cc)
    {
        cc.enabled = false;
        
        iTween.ScaleTo(gameObject, _optionsOut);

        safeFadeTo(_optionsOutAlpha);  

        _flagella = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if(child.name == "FBX_flagelPlayer" && child.gameObject.activeSelf)
            {
                _flagella.Add(child.gameObject);
            }
        }
        
        //1 wait sequence between flagella, pair of eyes disappearances
        //therefore #flagella + #pairs of eyes - 1
        int maxWaitSequences = _flagella.Count;
        
        //fractional elapsed time
        // 0<elapsed<maxWaitSequences
        float elapsed = 0.0f;

        for(int i=0; i<_flagella.Count; i++)
        {
            //to make flagella disappear
            float random = UnityEngine.Random.Range(0.0f,1.0f);
            yield return new WaitForSeconds(random*_respawnTimeS/maxWaitSequences);
            _flagella[i].SetActive(false);
            elapsed += random;
        }

        //to make eyes disappear
        float lastRandom = UnityEngine.Random.Range(0.0f,1.0f);
        yield return new WaitForSeconds(lastRandom*_respawnTimeS/maxWaitSequences);
        elapsed += lastRandom;        
        enableEyes(false);

        yield return new WaitForSeconds((maxWaitSequences-elapsed)*_respawnTimeS/maxWaitSequences);
        
        respawnCoroutine(cc);
    }

    private void enableEyes(bool enable)
    {
        foreach(MeshRenderer mr in transform.FindChild("FBX_eyePlayer").GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = enable;
        }
    }
    
    void respawnCoroutine(CellControl cc)
    {
        enableEyes(true);
        
        foreach(GameObject flagellum in _flagella)
        {
            flagellum.SetActive(true);
        }
        _flagella.Clear();

        iTween.ScaleTo(gameObject, _optionsIn);
        safeFadeTo(_optionsInAlpha);  
        
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
        _ambientLighting.ResetLighting();

        StartCoroutine(popEffectCoroutine(savedCell));
    }
    
    IEnumerator popEffectCoroutine(SavedCell savedCell)
    {
        yield return new WaitForSeconds(_popEffectTimeS);
        if(null != savedCell) {
            savedCell.setCollidable(true);
        } else {
            Logger.Log("Hero::popEffectCoroutine unexpected null savedCell", Logger.Level.WARN);
        }
    }
}