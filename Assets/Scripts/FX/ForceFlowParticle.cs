using UnityEngine;

public class ForceFlowParticle : MonoBehaviour {

  public float force;
  public float period;
  public float startPhase;
  public ParticleSystem system;
  
  private ParticleSystem.EmissionModule _em;
  private float _timeDelta;
  private float _timeAtLastFrame = 0f;
  private float _timeAtCurrentFrame = 0f;
  private float _deltaTime = 0f;
  private bool _isOn = true;

	void OnParticleCollision(GameObject obj){
    Hero cellia = obj.GetComponent<Hero>();
    if(cellia){
      Rigidbody body = cellia.GetComponent<Rigidbody>();
      if (body){
        Vector3 push = this.transform.rotation * new Vector3(0,0,1);
        body.AddForce((push * force));
      }
    }
  }

  void Toggle()
  {
    _isOn = !_isOn;
    if(_isOn)
    {
      _em.enabled = true;
    }
    else
    {
      _em.enabled = false;
    }
  }

  void Update()
  {
    _timeAtCurrentFrame = Time.realtimeSinceStartup;
    _deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
    if(_deltaTime > _timeDelta) {
      Toggle();
      _timeAtLastFrame = _timeAtCurrentFrame;
    }
  }

  void Start()
  {
    _timeDelta = period;
    _timeAtLastFrame = - startPhase * period;
    _em = system.emission;
  }

}
