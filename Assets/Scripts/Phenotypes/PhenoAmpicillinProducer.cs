using UnityEngine;
using System.Collections.Generic;

public class PhenoAmpicillinProducer : MonoBehaviour
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    public const string gameObjectName = Hero.gameObjectName;
    private static PhenoAmpicillinProducer _instance;
    public static PhenoAmpicillinProducer get()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find(gameObjectName);
            if (null != go)
            {
                _instance = go.GetComponent<PhenoAmpicillinProducer>();
                if (null != _instance)
                {
                    _instance.initialize();
                }
                else
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
        Logger.Log("PhenoAmpicillinProducer::Awake", Logger.Level.INFO);
        PhenoAmpicillinProducer.get();
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

    private bool _isActive = false;
    public bool isActive
    {
        get
        {
            return _isActive;
        }
        set
        {
            _isActive = value;
            _timeUntilNextSpawn = _timeBetweenSpawns;
        }
    }

    private void initialize()
    {
    }

    // TODO replace using event system
    public bool onEquippedDevice(Device device)
    {
        bool contains = containsAmpicillinBrick(device);
        isActive = isActive || contains;

        return contains;
    }
    public void onUnequippedDevice(Device device)
    {
        isActive = Equipment.get().exists(d => containsAmpicillinBrick(d));
    }
    private bool containsAmpicillinBrick(Device device)
    {
        return (_brickName == device.getFirstGeneBrickName());
    }

    void FixedUpdate()
    {
        if (isActive)
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

    public void resetClouds()
    {
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
