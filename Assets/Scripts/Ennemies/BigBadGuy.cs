using UnityEngine;
using System.Collections;

public class BigBadGuy : MonoBehaviour
{
  
    public int life = 50;
    public float shrinkSpeed = 3;
    public Hero hero;
    private float step;
    //TODO extract to config file
    private float _dpt = 0.5f;
    public iTweenPath _iTP;
  
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
}
