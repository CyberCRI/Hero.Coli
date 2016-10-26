using UnityEngine;

public class ForceFlowParticle : MonoBehaviour
{

    public float force;
    public float period;
    public float startPhase;
    public ParticleSystem system;

    private ParticleSystem.EmissionModule _em;
    private float _deltaTime = 0f;
    private bool _isOn = true;

    void OnParticleCollision(GameObject obj)
    {
        Hero cellia = obj.GetComponent<Hero>();
        if (cellia)
        {
            Rigidbody body = cellia.GetComponent<Rigidbody>();
            if (body)
            {
                Vector3 push = this.transform.rotation * new Vector3(0, 0, 1);
                body.AddForce((push * force));
            }
        }
    }

    void toggle()
    {
        _isOn = !_isOn;
        if (_isOn)
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
        _deltaTime += Time.deltaTime;
        if (_deltaTime > period)
        {
            toggle();
            _deltaTime = 0;
        }
    }

    void Start()
    {
        _deltaTime = -startPhase * period;
        _em = system.emission;
    }

}
