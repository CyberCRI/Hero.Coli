using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Hero : MonoBehaviour
{

    public const string playerTag = "Player";

    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public const string gameObjectName = "Perso";
    protected static Hero _instance;
    public static Hero get()
    {
        if (_instance == null)
        {
            Debug.LogWarning("Hero::get called too early");
            GameObject go = GameObject.Find(gameObjectName);
            if (null != go)
            {
                _instance = go.GetComponent<Hero>();
                if(null == _instance)
                {
                    Debug.LogError("component Hero of " + gameObjectName + " not found");
                }
            }
            else
            {
                Debug.LogError(gameObjectName + " not found");
            }
        }
        return _instance;
    }
    void Awake()
    {
        // Debug.Log(this.GetType() + " Awake");
        if((_instance != null) && (_instance != this))
        {            
            Debug.LogError(this.GetType() + " has two running instances");
        }
        else
        {
            _instance = this;
            initializeIfNecessary();
        }
    }

    private bool _isInitialized = false;
    private void initializeIfNecessary()
    {
        if(!_isInitialized)
        {
            _instance = this;
            //???
            //gameObject.SetActive(true);
            _isAlive = true;

            //LinkedList<Medium> mediums = ReactionEngine.get ().getMediumList();
            _isInitialized = true;
        }
    }

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
       _instance = (_instance == this) ? null : _instance;
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");

        _ambientLighting = this.GetComponent<AmbientLighting>();
        _medium = ReactionEngine.getMediumFromId(Hero.mediumId, ReactionEngine.get().getMediumList());
        _maxMediumEnergy = _medium.getMaxEnergy();
        _medium.setEnergy(_maxMediumEnergy);
        _energy = _medium.getEnergy() / _maxMediumEnergy;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    private const float _originOffsetY = 41.11548f;
    [SerializeField]
    private AmbientLighting _ambientLighting;
    [SerializeField]
    private GameObject _savedCellPrefab;

    public LifeLogoAnimation lifeAnimation;
    public EnergyLogoAnimation energyAnimation;
    private Medium _medium;
    public Medium medium
    {
        get
        {
            return _medium;
        }
    }
    public const int mediumId = 1; 

    //Life
    private const float _life = 1f;
    private const float _lifeRegen = 0.1f;
    [SerializeField]
    private Life _lifeManager = new Life(_life, _lifeRegen);

    //Energy.
    private float _energy = 1f;
    private float _maxMediumEnergy = 1f;
    private float _energyBefore = 1f;
    private float _lowEnergyDpt = 3 * _lifeRegen;

    //Status
    private bool _pause;
    private bool _isAlive;
    private bool _isInjured;
    private bool _isBeingInjured;
    private bool _isRegen;
    private float _previousLife;

    //respawn
    private GameObject _lastCheckpoint = null;
    private GameObject _lastNewCell = null;


    private const float _respawnTimeS = 1.5f;
    private const float _disappearingTimeSRatio = 0.9f;
    private const float _disappearingTimeS = _disappearingTimeSRatio * _respawnTimeS;
    private float _popEffectTimeS = 1.0f;
    private const float _baseScale = 145.4339f;
    private static Vector3 _baseScaleVector = new Vector3(_baseScale, _baseScale, _baseScale);
    private static Vector3 _reducedScaleVector = 0.7f * _baseScaleVector;
    private List<GameObject> _flagella = new List<GameObject>();
    private const string checkpointSeparator = ".";
    private const string _keyLife = "KEY.LIFE";
    private const string _keyEnergy = "KEY.ENERGY";

    private Hashtable _optionsIn = iTween.Hash(
        "scale", _baseScaleVector,
        "time", 0.8f,
        "easetype", iTween.EaseType.easeOutElastic
        );

    private Hashtable _optionsOut = iTween.Hash(
        "scale", _reducedScaleVector,
        "time", _disappearingTimeS,
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
        "time", _disappearingTimeS,
        "easetype", iTween.EaseType.easeInQuint
        );
     
    public static bool isInjured
    {
        get
        {
            return _instance._isInjured;
        }
    }
    public static bool isBeingInjured
    {
        get
        {
            return _instance._isBeingInjured;
        }
    }

    public Life getLifeManager() { return _lifeManager; }

    public void kill()
    {
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

    public bool isAlive() { return _isAlive; }


    //getter and setter for energy
    public float getEnergy()
    {
        return _energy;
    }


    public void setEnergy(float energy)
    {
        if (energy > 1f)
        {
            energy = 1f;
        }
        else if (energy < 0)
        {
            energy = 0;
        }
        _medium.setEnergy(energy * _maxMediumEnergy);
        _energy = energy;
    }


    //energy in ReactionEngine scale (not in percent or ratio)
    public void subEnergy(float energy)
    {
        _medium.subEnergy(energy);
    }

    public void DisplayEnergyAnimation()
    {
        if (energyAnimation.isPlaying == false)
        {
            energyAnimation.Play();
        }
    }


    //Getter & setter for the life.
    public float getLife()
    {
        return _lifeManager.getLife();
    }

    public void setLife(float life)
    {
        _lifeManager.setLife(life);
    }

    public void addLife(float life)
    {
        _lifeManager.addVariation(life);
    }

    public void subLife(float life)
    {
        _lifeManager.addVariation(-life);
    }

    void Update()
    {
        if (!_pause)
        {
            _lifeManager.regen(Time.deltaTime);
            _energy = _medium.getEnergy() / _maxMediumEnergy;

            if (GameStateController.isShortcutKey(_keyLife, true))
            {
                _lifeManager.addVariation(1f);
            }
            if (GameStateController.isShortcutKey(_keyEnergy, true))
            {
                setEnergy(1f);
            }

            // damage in case of low energy
            if (_energy <= 0.05f)
            {
                _lifeManager.addVariation(-Time.deltaTime * _lowEnergyDpt);
            }


            // Life animation when life is reducing
            _isBeingInjured = _lifeManager.getVariation() < 0; 
            if (_isBeingInjured)
            {
                if (!lifeAnimation.isPlaying)
                {
                    lifeAnimation.Play();
                }
            }

            // Energy animation when energy is reducing
            if (_energy < _energyBefore)
            {
                DisplayEnergyAnimation();
            }
            _energyBefore = _energy;


            _lifeManager.applyVariation();
            if (_lifeManager.getLife() == 0f && _isAlive)
            {
                _isAlive = false;
                _ambientLighting.setDead();

                RedMetricsManager.get().sendEvent(TrackingEvent.DEATH, null, getLastCheckpointName());
                StartCoroutine(RespawnCoroutine());
            }
            float life = _lifeManager.getLife();
            if (_lifeManager.getLife() <= 0.95f)
            {
                if (!_isInjured)
                {
                    //_ambientLighting.SaveCurrentLighting();
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
                _ambientLighting.setInjured(life);
            }
            else if (_lifeManager.getLife() > 0.95f)
            {
                if (_isInjured)
                {
                    _isInjured = false;
                    if (!_ambientLighting.isBlackLight())
                    {
                        _ambientLighting.startReset();
                    }
                }
                else
                {
                    //_ambientLighting.SaveCurrentLighting();
                }
            }
        }
        _ambientLighting.setInjured(_lifeManager.getLife());
        _previousLife = _lifeManager.getLife();

    }

    public string getLastCheckpointName()
    {
        string levelName = MemoryManager.get().configuration.getGameMapName();
        string lastCheckpointName = null == _lastCheckpoint ? "" : _lastCheckpoint.name;
        string result = string.IsNullOrEmpty(lastCheckpointName) ? lastCheckpointName : levelName + checkpointSeparator + lastCheckpointName;
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
        if (null != item)
        {
            // Debug.Log("Hero::managePickUp collided with DNA! bit=" + item.getDNABit());
            item.pickUp();
            RedMetricsManager.get ().sendEvent(TrackingEvent.PICKUP, new CustomData(CustomDataTag.DNABIT, item.getDNABit().getInternalName()));
        }
    }

    // Manages sector when entering into the sector.
    void manageSector(Collider col)
    {
        Sector sector = col.GetComponent<Sector>();
        if (null != sector)
        {
            // Debug.Log("Hero::manageSector collided with sector=" + sector.ToString());
            sector.activate();
        }
    }

    void updateMapElements(int checkpointIndex)
    { }

    void setCurrentRespawnPoint(Collider col)
    {
        if (null != col.gameObject.GetComponent<RespawnPoint>()
             && (null == _lastCheckpoint
              || _lastCheckpoint.name != col.gameObject.name))
        {
            _lastCheckpoint = col.gameObject;
            duplicateCell();

            //RedMetrics reporting
            //TODO put equiped devices in customData of sendEvent
            RedMetricsManager.get().sendEvent(TrackingEvent.REACH);
        }
    }

    //TODO divide by 2 the chemicals
    void duplicateCell()
    {
        if (null != _lastNewCell)
        {
            Destroy(_lastNewCell);
        }
        else
        {
            ModalManager.setModal("FirstCheckpoint");
        }

        _lastNewCell = (GameObject)Instantiate(_savedCellPrefab, this.gameObject.transform.position, this.gameObject.transform.localRotation, this.transform.parent);

        SavedCell savedCell = (SavedCell)_lastNewCell.GetComponent<SavedCell>();
        savedCell.initialize(this);
        _lastNewCell.SetActive(true);

        StartCoroutine(popEffectCoroutine(savedCell));
    }

    void OnTriggerEnter(Collider collision)
    {
        managePickUp(collision);
        manageCheckpoint(collision);
        manageSector(collision);
    }

    void OnTriggerExit(Collider col)
    {
        manageCheckpoint(col);
    }

    //Respawn function after death
    IEnumerator RespawnCoroutine()
    {

        CellControl cc = GetComponent<CellControl>();
        GUITransitioner.get().mainBoundCamera.offset.y = _originOffsetY;

        yield return StartCoroutine(deathEffectCoroutine(cc));
    }

    public static void safeFadeTo(GameObject toFade, Hashtable fadeOptions)
    {
        //TODO find most robust method
        //GameObject body = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(). .transform.FindChild("body_perso");
        //GameObject body = gameObject.transform.FindChild("body_perso").gameObject;
        GameObject body = toFade.transform.FindChild("body_perso").transform.FindChild("body_perso").gameObject;
        if (null != body)
        {
            iTween.FadeTo(body, fadeOptions);
        }
        GameObject dna = toFade.transform.FindChild("dna_perso").transform.FindChild("body_perso").gameObject;
        if (null != dna)
        {
            iTween.FadeTo(dna, fadeOptions);
        }
    }

    private void safeFadeTo(Hashtable hash)
    {
        safeFadeTo(gameObject, hash);
    }

    IEnumerator deathEffectCoroutine(CellControl cc)
    {
        cc.enabled = false;

        iTween.ScaleTo(gameObject, _optionsOut);

        safeFadeTo(_optionsOutAlpha);

        _flagella.Clear();

        foreach (Transform child in transform)
        {
            if (child.name == "FBX_flagelPlayer" && child.gameObject.activeSelf)
            {
                _flagella.Add(child.gameObject);
            }
        }

        //1 wait sequence between flagella, pair of eyes disappearances
        //therefore #flagella + #pairs of eyes - 1
        int maxWaitSequences = _flagella.Count == 0 ? 1 : _flagella.Count;

        //fractional elapsed time
        // 0<elapsed<maxWaitSequences
        float elapsed = 0.0f;

        foreach (GameObject flagellum in _flagella)
        {
            //to make flagella disappear
            float random = UnityEngine.Random.Range(0.0f, 1.0f);
            yield return new WaitForSeconds(random * _respawnTimeS / maxWaitSequences);
            flagellum.SetActive(false);
            elapsed += random;
        }

        //to make eyes disappear
        float lastRandom = UnityEngine.Random.Range(0.0f, 1.0f);
        yield return new WaitForSeconds(lastRandom * _respawnTimeS / maxWaitSequences);
        elapsed += lastRandom;
        enableEyes(false);

        yield return new WaitForSeconds((maxWaitSequences - elapsed) * _respawnTimeS / maxWaitSequences);

        respawn(cc);
    }

    private void enableEyes(bool enable)
    {
        foreach (MeshRenderer mr in transform.FindChild("FBX_eyePlayer").GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = enable;
        }
    }

    void respawn(CellControl cc)
    {
        enableEyes(true);

        foreach (GameObject flagellum in _flagella)
        {
            flagellum.SetActive(true);
        }
        _flagella.Clear();

        iTween.ScaleTo(gameObject, _optionsIn);
        safeFadeTo(_optionsInAlpha);

        // reset all pushable rocks
        cc.enabled = true;
        foreach (PushableBox box in FindObjectsOfType(typeof(PushableBox)))
        {
            box.resetPos();
        }

        MineManager.get().resetAllMines();
        PhenoAmpicillinProducer.get().reset();
        GetComponent<PhenoFickContact>().onDied();
        medium.resetMolecules();

        SavedCell savedCell = null;
        if (null != _lastNewCell)
        {
            savedCell = (SavedCell)_lastNewCell.GetComponent<SavedCell>();
            savedCell.resetCollisionState();
            gameObject.transform.position = _lastNewCell.transform.position;
            gameObject.transform.rotation = _lastNewCell.transform.rotation;
        }

        _isAlive = true;
        cc.reset();
        setLife(1f);
        setEnergy(1f);

        Molecule ampicillin = ReactionEngine.getMoleculeFromName(PhenoToxic.ampicillinMoleculeName, _medium.getMolecules());
        if (null != ampicillin)
        {
            ampicillin.setNewConcentration(0);
        }

        _ambientLighting.startReset();

        StartCoroutine(popEffectCoroutine(savedCell));
    }

    IEnumerator popEffectCoroutine(SavedCell savedCell)
    {
        yield return new WaitForSeconds(_popEffectTimeS);
        if (null != savedCell)
        {
            savedCell.setCollidable(true);
        }
        else
        {
            Debug.LogWarning("Hero::popEffectCoroutine unexpected null savedCell");
        }
    }
}