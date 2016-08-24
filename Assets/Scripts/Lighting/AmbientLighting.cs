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
    [SerializeField]
    private Renderer _backgroundBlood;
    private float _originalDirectionalIntensity;
    private float _originalSpotLightIntensity;
    private float _originalPhenoLightIntensity;
    private PulsingLight _ampicillinPulsingLight;
    private float _originMaxPulse;
    private float _originMinPulse;
    private Vector3 _originGradient;
    private Vector3 _limitGradient;
    private Hero _hero;
    private bool _blackLight = false;
    private bool _injured = false;
    private bool _dead = false;

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

        float heroLife = _hero.getLife();
        Color color = _backgroundBlood.material.color;

        if (heroLife <= 0.95 && heroLife != 0)
        {
            if (_injured == false)
            {
                _injured = true;
                _originalDirectionalIntensity = _directionaleLight.intensity;
                _originalPhenoLightIntensity = _directionaleLight.intensity;
                _originalSpotLightIntensity = _spotLight.intensity;
            }
            color.a = 0.5f - heroLife;
            if (_blackLight == false)
            {
                ChangeLightIntensity(_directionaleLight, _originalDirectionalIntensity * heroLife);
                ChangeLightIntensity(_phenoLight, _originalPhenoLightIntensity * heroLife);
                ChangeLightIntensity(_spotLight, _originalSpotLightIntensity * heroLife);
            }
        }
        else if (heroLife == 0)
        {
            color.a = 1f;
            _dead = true;
        }
        else if (_dead = true && heroLife >= 0)
        {
            _injured = false;
            color.a = 0f;
            ChangeLightIntensity(_directionaleLight, _originalDirectionalIntensity);
            ChangeLightIntensity(_phenoLight, _originalPhenoLightIntensity);
            ChangeLightIntensity(_spotLight, _originalSpotLightIntensity);

        }
        else
        {
            color.a = 0f;
            _injured = false;
            ChangeLightIntensity(_directionaleLight, _originalDirectionalIntensity);
            ChangeLightIntensity(_phenoLight, _originalPhenoLightIntensity);
            ChangeLightIntensity(_spotLight, _originalSpotLightIntensity);
        }
        _backgroundBlood.material.color = color;
    }

    void Start()
    {
        _originalDirectionalIntensity = _directionaleLight.intensity;
        ChangeLightProperty(_phenoLight, _color[1], 24, 4);
        ChangeLightProperty(_spotLight, _color[1], 57.5f, 5.3f);
        _ampicillinPulsingLight = GameObject.Find("AmpicillinPulsingLight").GetComponent<PulsingLight>();
        _originMaxPulse = _ampicillinPulsingLight.GetMaxIntensityValue();
        _originMinPulse = _ampicillinPulsingLight.GetMinIntensityValue();
        _hero = this.GetComponent<Hero>();
        Color color = _backgroundBlood.material.color;
        color.a = 0;
        _backgroundBlood.material.color = color;
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
