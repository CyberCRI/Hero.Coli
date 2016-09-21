using UnityEngine;

public class PhenoAmpicillinProducer : MonoBehaviour
{
    private const string _brickName = "AMPI";
    private const string _ampicillinTag = "Ampicillin";
    private const float _timeBetweenSpawns = 2.0f;
    [SerializeField]
    private GameObject _smallAmpicillinCloud;
    private float _timeUntilNextSpawn = 2.0f;
    private Hero _hero;
    private Vector3 _scale = new Vector3(.5f, 1f, 1f);
    private float _radius = 2f;

    private bool _isActive = true;
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

    void Update()
    {
        if (isActive)
        {
            _timeUntilNextSpawn -= Time.deltaTime;
            if (_timeUntilNextSpawn <= 0)
            {
                spawnCloud();
                _timeUntilNextSpawn = _timeBetweenSpawns;
            }
        }
    }

    private void spawnCloud()
    {
        // don't spawn new clouds next to other clouds
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radius);
        Collider match = System.Array.Find(hitColliders, col => col.gameObject.tag == _ampicillinTag);
        if (!match)
        {
            Debug.Log("spawn at " + Time.time + " because _timeUntilNextSpawn = " + _timeUntilNextSpawn);
            if (null == _hero)
            {
                _hero = Hero.get();
            }
            GameObject cloud = Instantiate(_smallAmpicillinCloud, _hero.transform.position, _hero.transform.rotation * Quaternion.AngleAxis(90, Vector3.up)) as GameObject;
            cloud.transform.localScale = _scale;
        }
    }
}
