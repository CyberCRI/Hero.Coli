using UnityEngine;

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
    [SerializeField]
    private LightingSave _lightingSave;
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
    private Color _alphaColor;

    /*Values : 
    Phenolight : 
    GFP classic = _phenoLight, _color[1] (8,255,0,0), 24, 4

    SpotLight : 
    GFP Classic = _spotLight, _color[2] (105,255,118,255), 57.5, 5.3
    
    */

    public void changeLightColor(Light light, Color color)
    {
        light.color = color;
    }

    public void changeLightRange(Light light, float range)
    {
        light.range = range;
    }

    public void changeLightIntensity(Light light, float intensity)
    {
        light.intensity = intensity;
    }

    public void changeLightProperty(Light light, Color color, float range, float intensity)
    {
        light.color = color;
        light.range = range;
        light.intensity = intensity;
    }

    public void enableLight(Light light, bool value)
    {
        light.enabled = value;
    }



    ////////////Tests


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            changeLightProperty(_phenoLight, _color[1], 24, 4);
            changeLightProperty(_spotLight, _color[1], 57.5f, 5.3f);
        }
    }

    void Start()
    {
        _originalDirectionalIntensity = _directionaleLight.intensity;
        changeLightProperty(_phenoLight, _color[1], 24, 4);
        changeLightProperty(_spotLight, _color[1], 57.5f, 5.3f);
        _ampicillinPulsingLight = GameObject.Find("AmpicillinPulsingLight").GetComponent<PulsingLight>();
        _originMaxPulse = _ampicillinPulsingLight.GetMaxIntensityValue();
        _originMinPulse = _ampicillinPulsingLight.GetMinIntensityValue();
        _hero = this.GetComponent<Hero>();
        Color color = _backgroundBlood.material.color;
        color.a = 0;
        _backgroundBlood.material.color = color;
        saveCurrentLighting();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "BlackLight")
        {
            _blackLight = true;
            _originGradient = col.transform.position;
            _limitGradient = col.transform.GetChild(0).transform.position;
            _ampicillinPulsingLight.TweekRangeIntensity(0, 0.01f);
        }
        if (col.tag == "BlackLightInverse")
        {
            _blackLight = true;
            _originGradient = col.transform.position;
            _limitGradient = col.transform.GetChild(0).transform.position;
            _ampicillinPulsingLight.TweekRangeIntensity(0, 0.01f);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "BlackLight")
        {
            if (this.transform.position.x <= col.transform.position.x)
            {
                _blackLight = false;
                changeLightIntensity(_directionaleLight, _originalDirectionalIntensity);
                _ampicillinPulsingLight.TweekRangeIntensity(_originMinPulse, _originMaxPulse);
                startReset();
            }
        }
        if (col.tag == "BlackLightInverse")
        {
            if (this.transform.position.x >= col.transform.position.x)
            {
                _blackLight = false;
                changeLightIntensity(_directionaleLight, _originalDirectionalIntensity);
                _ampicillinPulsingLight.TweekRangeIntensity(_originMinPulse, _originMaxPulse);
                startReset();
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "BlackLight")
        {
            float distanceMax = Vector3.Distance(_originGradient, _limitGradient);
            float distance = Vector3.Distance(_originGradient, this.transform.position);
            changeLightIntensity(_directionaleLight, 1 - (distance / distanceMax));
            Color color = _background.GetComponent<Renderer>().material.color;
            color.a = 1 - (distance / distanceMax);
            _background.GetComponent<Renderer>().material.color = color;
        }
        if (col.tag == "BlackLightInverse")
        {
            float distanceMax = Vector3.Distance(_originGradient, _limitGradient);
            float distance = Vector3.Distance(_originGradient, this.transform.position);
            changeLightIntensity(_directionaleLight, distance / distanceMax);
            Color color = _background.GetComponent<Renderer>().material.color;
            color.a = distance / distanceMax;
            _background.GetComponent<Renderer>().material.color = color;
        }
    }

    public void setInjured(float life)
    {
        if (_blackLight == false)
        {
            changeLightIntensity(_directionaleLight, _lightingSave.GetOriginDirectional() * life);
            changeLightIntensity(_phenoLight, _lightingSave.GetOriginPheno() * life);
            changeLightIntensity(_spotLight, _lightingSave.GetOriginSpot() * life);
        }
        _alphaColor = _backgroundBlood.material.color;
        _alphaColor.a = 0.5f - life;
        _backgroundBlood.material.color = _alphaColor;
    }

    public void setDead()
    {
        _alphaColor = _backgroundBlood.material.color;
        _alphaColor.a = 1f;
        _backgroundBlood.material.color = _alphaColor;
    }

    public void startReset()
    {
        _lightingSave.GetOriginForReset();
    }

    public void resetLighting(float directional, float pheno, float spotLight)
    {
        _alphaColor = _backgroundBlood.material.color;
        _alphaColor.a = 0f;
        _backgroundBlood.material.color = _alphaColor;
        _alphaColor = _background.GetComponent<Renderer>().material.color;
        _alphaColor.a = 1f;
        _background.GetComponent<Renderer>().material.color = _alphaColor;
        changeLightIntensity(_directionaleLight, directional);
        changeLightIntensity(_phenoLight, pheno);
        changeLightIntensity(_spotLight, spotLight);
    }

    public void saveCurrentLighting()
    {
        _originalDirectionalIntensity = _directionaleLight.intensity;
        _originalPhenoLightIntensity = _phenoLight.intensity;
        _originalSpotLightIntensity = _spotLight.intensity;
    }

    public bool isBlackLight()
    {
        return _blackLight;
    }
}
