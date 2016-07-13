using UnityEngine;
using System.Collections;

public class PulsingLight : MonoBehaviour {

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


    // Use this for initialization
    void Start () {
        _ownLight = this.GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_way == false)
        {
            if (_tweekIntensity == true)
            {
                _ownLight.intensity += _speedIntensity / 100;
            }
            if (_tweekRange == true)
            {
                _ownLight.range += _speedRange / 100;
            }
            if (_ownLight.intensity >= _maxIntensity && _tweekIntensity == true)
            {
                _way = !_way;
            }
            if (_ownLight.range >= _maxRange && _tweekRange == true)
            {
                _way = !_way;
            }
        }
        else
        {
            if (_tweekIntensity == true)
            {
                _ownLight.intensity -= _speedIntensity / 100;
            }
            if (_tweekRange == true)
            {
                _ownLight.range -= _speedRange / 100;
            }
            if (_ownLight.intensity <= _minIntensity && _tweekIntensity == true)
            {
                _way = !_way;
            }
            if (_ownLight.range <= _minRange && _tweekRange == true)
            {
                _way = !_way;
            }
        }

	}
}
