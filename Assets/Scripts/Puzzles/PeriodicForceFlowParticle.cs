using UnityEngine;

public class PeriodicForceFlowParticle : ForceFlowParticle
{
    [SerializeField]
    protected ParticleSystem _system;
    [SerializeField]
    private float period;
    [SerializeField]
    private float startPhase;
    [SerializeField]
    private float _startDelay;
    [SerializeField]
    private bool _isOnAtStart = true;

    private float _deltaTime = 0f;
    private bool _isOn;

    void set()
    {
        if (_isOn)
        {
            _system.Play();
        }
        else
        {
            _system.Stop(true);
        }
    }

    void toggle()
    {
        _isOn = !_isOn;
        set();
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
        _deltaTime = startPhase * period - _startDelay;
        _isOn = _isOnAtStart;
        set();
    }
}