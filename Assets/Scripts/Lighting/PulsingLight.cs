using UnityEngine;
using System.Collections;

public class PulsingLight : MonoBehaviour
{

    [SerializeField]
    private bool _tweakIntensity = true;
    [SerializeField]
    private bool _tweakRange = false;
    private Light _ownLight;
    [SerializeField]
    private float _intensitySpeed;
    [SerializeField]
    private float _rangeSpeed;
    private bool _isLightIncreasing = true;
    [SerializeField]
    private float _minIntensity = 1;
    [SerializeField]
    private float _maxIntensity = 2;
    [SerializeField]
    private float _minRange;
    [SerializeField]
    private float _maxRange;

    void Awake()
    {
        _ownLight = this.GetComponent<Light>();

        if (_tweakIntensity)
            _ownLight.intensity = _minIntensity;
        if (_tweakRange)
            _ownLight.range = _minRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isLightIncreasing)
        {
            if (_tweakIntensity)
            {
                _ownLight.intensity += _intensitySpeed * Time.deltaTime;
                if (_ownLight.intensity > _maxIntensity)
                {
                    _ownLight.intensity = _maxIntensity;
                    _isLightIncreasing = !_isLightIncreasing;
                }
            }
            if (_tweakRange)
            {
                _ownLight.range += _rangeSpeed * Time.deltaTime;
                if (_ownLight.range > _maxRange)
                {
                    _ownLight.range = _maxRange;
                    _isLightIncreasing = !_isLightIncreasing;
                }
            }
        }
        else
        {
            if (_tweakIntensity)
            {
                _ownLight.intensity -= _intensitySpeed * Time.deltaTime;
                if (_ownLight.intensity < _minIntensity)
                {
                    _ownLight.intensity = _minIntensity;
                    _isLightIncreasing = !_isLightIncreasing;
                }
            }
            if (_tweakRange)
            {
                _ownLight.range -= _rangeSpeed * Time.deltaTime;
                if (_ownLight.range < _minRange)
                {
                    _ownLight.range = _minRange;
                    _isLightIncreasing = !_isLightIncreasing;
                }
            }
        }
    }

    public void TweekRangeIntensity(float min, float max)
    {
        _minIntensity = min;
        _maxIntensity = max;
    }

    public float GetMaxIntensityValue()
    {
        return _maxIntensity;
    }

    public float GetMinIntensityValue()
    {
        return _minIntensity;
    }

}
