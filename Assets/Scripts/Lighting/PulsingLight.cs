using UnityEngine;
using System.Collections;

public class PulsingLight : MonoBehaviour
{

    [SerializeField]
    private bool _tweekIntensity = true;
    [SerializeField]
    private bool _tweekRange = false;
    private Light _ownLight;
    [SerializeField]
    private float _speedIntensity;
    [SerializeField]
    private float _speedRange;
    private bool _way = false;
    [SerializeField]
    private float _minIntensity = 1;
    [SerializeField]
    private float _maxIntensity = 2;
    [SerializeField]
    private float _minRange;
    [SerializeField]
    private float _maxRange;

    private float _speedIntensityPercent;
    private float _speedRangePercent;

    void Awake()
    {
        _speedIntensityPercent = _speedIntensity / 100;
        _speedRangePercent = _speedRange / 100;
        _ownLight = this.GetComponent<Light>();

        if (_tweekIntensity)
            _ownLight.intensity = _minIntensity;
        if (_tweekRange)
            _ownLight.range = _minRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_way)
        {
            if (_tweekIntensity)
            {
                _ownLight.intensity += _speedIntensityPercent;
                if (_ownLight.intensity >= _maxIntensity)
                {
                    _way = !_way;
                }
            }
            if (_tweekRange)
            {
                _ownLight.range += _speedRangePercent;
                if (_ownLight.range >= _maxRange)
                {
                    _way = !_way;
                }
            }
        }
        else
        {
            if (_tweekIntensity)
            {
                _ownLight.intensity -= _speedIntensityPercent;
                if (_ownLight.intensity <= _minIntensity)
                {
                    _way = !_way;
                }
            }
            if (_tweekRange)
            {
                _ownLight.range -= _speedRangePercent;
                if (_ownLight.range <= _minRange)
                {
                    _way = !_way;
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
