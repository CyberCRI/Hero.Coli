using UnityEngine;
using System.Collections;

public class BigBadGuy : MonoBehaviour
{
    private int _life = 50;
    private Character _character;
    private float _step;
    public iTweenPath _iTP;
    [SerializeField]
    private float _deathSpeed = 1f;
    [SerializeField]
    private GameObject _deadBigBadGuy;
    private bool _isInjured = false;
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
    private GameObject _corpse;

    void Awake()
    {
        _iTP = this.gameObject.GetComponent<iTweenPath>();
        EnemiesManager.register(this);
    }

    void Start()
    {
        _step = transform.localScale.x / _life;
        _slowDown = this.GetComponent<slowDown>();
    }

    void Update()
    {
        if (!_isDying)
        {
            if (_isSleeping)
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

            if (_lightIntensityIsIncreasing)
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

    public void pause(bool isPause)
    {
        if (isPause && null != _iTP && _iTP.IsInvoking())
        {
            iTween.Pause(gameObject);
        }
        else if (!isPause && null != _iTP && !_iTP.IsInvoking())
        {
            iTween.Resume(gameObject);
        }
    }

    // void OnParticleCollision(GameObject obj)
    // {
    //     AmpicillinCollider collider = obj.GetComponent<AmpicillinCollider>();
    //     if (null != collider)
    //     {
    //         Vector3 newScale = Vector3.Max(transform.localScale - new Vector3(_step, _step, _step), Vector3.zero);
    //         transform.localScale = newScale;
    //         _life--;

    //         if (_life == 0)
    //         {
    //             EnemiesManager.unregister(this);
    //             Destroy(gameObject);
    //         }
    //     }
    // }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider)
        {
            Character character = col.gameObject.GetComponent<Character>();
            if (null != character)
            {
                // Debug.Log(this.GetType() + " OnCollisionEnter hit character");
                character.kill(new CustomData(CustomDataTag.SOURCE, CustomDataValue.ENEMY.ToString()));
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Ampicillin")
        {
            if (!_isInjured)
            {
                float deathSpeed = _deathSpeed * Random.Range(0.5f, 2f);
                StartCoroutine(die(this.GetComponent<slowDown>(), deathSpeed));
                _isInjured = true;
            }
        }
    }

    IEnumerator die(slowDown slowDown, float deathSpeed)
    {
        _isDying = true;
        while (slowDown.percentage > 0)
        {
            slowDown.percentage -= Time.deltaTime * deathSpeed;
            yield return null;
        }
        setDead();
        yield return null;
    }

    public void wakeUp(bool value)
    {
        _isSleeping = !value;
    }

    public void setDead()
    {
        // Debug.Log(this.GetType() + " setDead");
        if (null == _corpse)
        {
            // Debug.Log(this.GetType() + " null == _corpse");
            _corpse = (GameObject)Instantiate(_deadBigBadGuy, Vector3.zero, this.transform.rotation);
            _corpse.transform.SetParent(this.transform.parent);
        }
        // else
        // {
        //     Debug.Log(this.GetType() + " null != _corpse");
        // }
        _corpse.transform.position = this.transform.position;
        this.gameObject.SetActive(false);
    }
}
