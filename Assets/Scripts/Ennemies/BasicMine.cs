using UnityEngine;

public class BasicMine : ResettableMine
{
    private bool _detonated = false;
    private float _x;
    private float _z;

    private bool _isReseting = false;

    private GameObject _particleSystem;
    private bool _first = true;

    public void detonate(bool reseting)
    {
        Debug.Log("self " + this.gameObject.name + " detonates");
        _isReseting = reseting;
        MineManager.get().detonate(this);
        _detonated = true;
    }

    public bool isDetonated() { return _detonated; }
    
    // Use this for initialization
    void Start()
    {
        //transform.localScale = Vector3.zero;
        _particleSystem = Resources.Load("ExplosionParticleSystem") as GameObject;
        _x = transform.position.x;
        _z = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void autoReset()
    {
        if (_isReseting && _detonated)
        {
            Debug.LogWarning("MINE " + mineName + " ASKS FOR RESETTING");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
		Debug.Log("colliding with " + collision.gameObject.name);
        if (collision.gameObject.name == "Perso")
        {
            detonate(true);
            collision.gameObject.GetComponent<Hero>().getLifeManager().setSuddenDeath(true);
        }
        else if (collision.gameObject.tag == "NPC")
        {
            detonate(false);
        }
    }

    void OnDestroy()
    {
        Debug.Log("11");
        _particleSystem = Resources.Load("ExplosionParticleSystem") as GameObject;
        GameObject instance = Instantiate(_particleSystem, new Vector3(this.transform.position.x, this.transform.position.y + 10, this.transform.position.z), this.transform.rotation) as GameObject;
    }

}
