using UnityEngine;
using System.Collections.Generic;

public class PhenoAmpicillinProducer : MonoBehaviour
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = Hero.gameObjectName;
    private static PhenoAmpicillinProducer _instance;
    public static PhenoAmpicillinProducer get()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find(gameObjectName);
            if (null != go)
            {
                _instance = go.GetComponent<PhenoAmpicillinProducer>();
                if (null == _instance)
                {
                    Debug.LogError("Component PhenoAmpicillinProducer of GameObject " + gameObjectName + " not found");
                }
            }
            else
            {
                Debug.LogWarning("GameObject " + gameObjectName + " not found");
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

    void OnDestroy()
    {
        // Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
       _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;  
    private void initializeIfNecessary()
    {
        if(!_initialized)
        {
            _initialized = true;
        }
    }

    void Start()
    {
        // Debug.Log(this.GetType() + " Start");
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    private const string _brickName = "AMPI";
    private const string _ampicillinTag = "Ampicillin";
    private const float _timeBetweenSpawns = 0.5f;
    [SerializeField]
    private GameObject _smallAmpicillinCloud;
    private float _timeUntilNextSpawn = 2.0f;
    private Hero _hero;
    private Vector3 _scale = new Vector3(.5f, 1f, 1f);
    private float _radius = 2f;
    private bool _isInCollision = false;
    private List<GameObject> _clouds = new List<GameObject>();

    private bool _isSpawningAmpicillin = false;
    public bool isSpawningAmpicillin
    {
        get
        {
            return _isSpawningAmpicillin;
        }
        set
        {
            _isSpawningAmpicillin = value;
            _timeUntilNextSpawn = _timeBetweenSpawns;
        }
    }

    // TODO replace using event system
    public bool onEquippedDevice(Device device)
    {
        bool contains = containsAmpicillinBrick(device);
        isSpawningAmpicillin = isSpawningAmpicillin || contains;
        return contains;
    }
    public void onUnequippedDevice(Device device)
    {
        isSpawningAmpicillin = Equipment.get().exists(d => containsAmpicillinBrick(d));
    }
    private bool containsAmpicillinBrick(Device device)
    {
        bool result = (_brickName == device.getFirstGeneBrickName());
        return result;
    }

    void FixedUpdate()
    {
        if (isSpawningAmpicillin)
        {
            _timeUntilNextSpawn -= Time.fixedDeltaTime;
            if (_timeUntilNextSpawn <= 0)
            {
                spawnCloud();
                _timeUntilNextSpawn = _timeBetweenSpawns;
            }
        }
        _isInCollision = false;
    }

    public void reset()
    {
        Equipment.get().removeAll(d =>  containsAmpicillinBrick(d));
        foreach(GameObject cloud in _clouds)
        {
            Destroy(cloud);
        }
        _clouds.Clear();
    } 

    private void checkAmpicillinCollision(Collider collider)
    {
        if(!_isInCollision && (_smallAmpicillinCloud.tag == collider.gameObject.tag))
        {
            _isInCollision = true;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        checkAmpicillinCollision(collider);
    }
    void OnTriggerStay(Collider collider)
    {
        checkAmpicillinCollision(collider);
    }

    private void spawnCloud()
    {
        if(!_isInCollision)
        {
            if (null == _hero)
            {
                _hero = Hero.get();
            }
            GameObject cloud = Instantiate(_smallAmpicillinCloud, _hero.transform.position, _hero.transform.rotation * Quaternion.AngleAxis(90, Vector3.up)) as GameObject;
            cloud.transform.localScale = _scale;
            _clouds.Add(cloud);
        }
    }
}
