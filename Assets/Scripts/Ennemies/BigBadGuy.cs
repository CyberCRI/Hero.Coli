using UnityEngine;
using System.Collections;

public class BigBadGuy : MonoBehaviour
{
  
    public int life = 50;
    public float shrinkSpeed = 3;
    public Hero hero;
    private float step;
    //TODO extract to config file
    private float _dpt = 2f;
    public iTweenPath _iTP;
    [SerializeField]
    private float _deathSpeed = 1f;
    [SerializeField]
    private GameObject _deadBigBadGuy;
    private bool _injured = false;
    [SerializeField]
    private bool _isSleeping = false;
    [SerializeField]
    private Light _pointLight;
    private float _maxPointLightIntensity;
    private float _minPointLightIntensity;
    private bool _lightIntensityIsIncreasing = false;
    private float _blinkSpeed;
    private slowDown _slowDown;
    private bool _isDying = false;

    void Awake ()
    {
        _iTP = this.gameObject.GetComponent<iTweenPath> ();
        EnemiesManager.register(this);
    }

    void Start ()
    {
        step = transform.localScale.x / life;
        _slowDown = this.GetComponent<slowDown>();
    }

    void Update()
    {
        if (_isDying == false)
        {
            if (_isSleeping == true)
            {
                _maxPointLightIntensity = 0.5f;
                _minPointLightIntensity = 0f;
                _blinkSpeed = 0.25f;
                _slowDown.percentage = 20f;
            }
            else
            {
                _maxPointLightIntensity = 2.5f;
                _minPointLightIntensity = 1.1f;
                _blinkSpeed = 2f;
                _slowDown.percentage = 100f;
            }

            if (_lightIntensityIsIncreasing == true)
            {
                if (_pointLight.intensity < _maxPointLightIntensity)
                {
                    _pointLight.intensity += Time.deltaTime * _blinkSpeed;
                }
                else
                {
                    _lightIntensityIsIncreasing = false;
                }
            }
            else
            {
                if (_pointLight.intensity > _minPointLightIntensity)
                {
                    _pointLight.intensity -= Time.deltaTime * _blinkSpeed;
                }
                else
                {
                    _lightIntensityIsIncreasing = true;
                }
            }
        }


    }
    
    public void Pause (bool isPause)
    {
        if (isPause && null != _iTP && _iTP.IsInvoking ()) {
            iTween.Pause (gameObject);
        } else if (!isPause && null != _iTP && !_iTP.IsInvoking ()) {
            iTween.Resume (gameObject);
        }
    }

    void OnParticleCollision (GameObject obj)
    {
        AmpicillinCollider collider = obj.GetComponent<AmpicillinCollider> ();
        if (null != collider) {
      
            Vector3 newScale = Vector3.Max (transform.localScale - new Vector3 (step, step, step), Vector3.zero);
            transform.localScale = newScale;
            life--;
      
            if (life == 0) {
                EnemiesManager.unregister(this);
                Destroy (gameObject);
            }
        }
    }

    void OnCollisionEnter (Collision col)
    {
        if (col.collider) {
            Hero hero = col.gameObject.GetComponent<Hero> ();
            if (null != hero) {
                Logger.Log ("BigBadGuy::OnCollisionEnter hit hero", Logger.Level.INFO);
                hero.subLife (_dpt);
            }
        }
    }
    
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Ampicillin")
        {
            if (_injured == false)
            {
                float deathSpeed = _deathSpeed * Random.Range(0.5f, 2f);
                StartCoroutine(BadGuyDeath(this.GetComponent<slowDown>(),deathSpeed));
                _injured = true;
            }
        }
    }

    IEnumerator BadGuyDeath(slowDown slowDown, float deathSPeed)
    {
        _isDying = true;
        while (slowDown.percentage > 0)
        {
            slowDown.percentage -= Time.deltaTime * deathSPeed;
            yield return null;
        }
        GameObject instance = (GameObject) Instantiate(_deadBigBadGuy, this.transform.position, this.transform.rotation);
        instance.transform.SetParent(this.transform.parent);
        this.gameObject.SetActive(false);
        yield return null;
    }

    public void WakeUp(bool value)
    {
        _isSleeping = !value;
    }
}
