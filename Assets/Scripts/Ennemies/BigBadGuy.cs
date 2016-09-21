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
    public bool _injured = false;

    void Awake ()
    {
        _iTP = this.gameObject.GetComponent<iTweenPath> ();
        EnemiesManager.register(this);
    }

    void Start ()
    {
        step = transform.localScale.x / life;
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
        Debug.Log(col.tag);
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
}
