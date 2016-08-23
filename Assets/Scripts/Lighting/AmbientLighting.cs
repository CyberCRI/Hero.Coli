using UnityEngine;
using System.Collections;

public class AmbientLighting : MonoBehaviour
{

    [SerializeField]
    private Light _directionaleLight;
    [SerializeField]
    private Light _phenoLight;
    [SerializeField]
    private Light _spotLight;
    [SerializeField]
    private Color[] _color;
    [SerializeField]
    private GameObject _background;
    private float _originalDirectionalIntensity;
    private PulsingLight _ampicillinPulsingLight;
    private float _originMaxPulse;
    private float _originMinPulse;
    private Vector3 _originGradient;
    private Vector3 _limitGradient;

    /*Values : 
    Phenolight : 
    GFP classic = _phenoLight, _color[1] (8,255,0,0), 24, 4

    SpotLight : 
    GFP Classic = _spotLight, _color[2] (105,255,118,255), 57.5, 5.3
    
    */

    public void ChangeLightColor(Light light, Color color)
    {
        light.color = color;
    }

    public void ChangeLightRange(Light light, float range)
    {
        light.range = range;
    }

    public void ChangeLightIntensity(Light light, float intensity)
    {
        light.intensity = intensity;
    }

    public void ChangeLightProperty(Light light, Color color, float range, float intensity)
    {
        light.color = color;
        light.range = range;
        light.intensity = intensity;
    }

    public void EnableLight(Light light, bool value)
    {
        light.enabled = value;
    }



    ////////////Tests


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ChangeLightProperty(_phenoLight, _color[1], 24, 4);
            ChangeLightProperty(_spotLight, _color[1], 57.5f, 5.3f);
        }
    }

    void Start()
    {
        _originalDirectionalIntensity = _directionaleLight.intensity;
        ChangeLightProperty(_phenoLight, _color[1], 24, 4);
        ChangeLightProperty(_spotLight, _color[1], 57.5f, 5.3f);
        _ampicillinPulsingLight = GameObject.Find("AmpicillinPulsingLight").GetComponent<PulsingLight>();
        _originMaxPulse = _ampicillinPulsingLight.GetMaxIntensityValue();
        _originMinPulse = _ampicillinPulsingLight.GetMinIntensityValue();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "BlackLight")
        {
            _originGradient = col.transform.position;
            _limitGradient = col.transform.GetChild(0).transform.position;
            _ampicillinPulsingLight.TweekRangeIntensity(0, 0.1f);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "BlackLight")
        {
            if (this.transform.position.x <= col.transform.position.x)
            {
                ChangeLightIntensity(_directionaleLight, _originalDirectionalIntensity);
                _ampicillinPulsingLight.TweekRangeIntensity(_originMinPulse, _originMaxPulse);
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "BlackLight")
        {
            float distanceMax = Vector3.Distance(_originGradient, _limitGradient);
            float distance = Vector3.Distance(_originGradient, this.transform.position);
            ChangeLightIntensity(_directionaleLight, 1 - (distance / distanceMax));
            Color color = _background.GetComponent<Renderer>().material.color;
            color.a = 1 - (distance / distanceMax);
            _background.GetComponent<Renderer>().material.color = color;
        }
    }
}
