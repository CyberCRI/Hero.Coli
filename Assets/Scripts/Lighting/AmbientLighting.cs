// #define DEV
using UnityEngine;

public class AmbientLighting : MonoBehaviour
{

    [SerializeField]
    private Light _directionalLight;
    [SerializeField]
    private Light _phenoLight, _spotLight, _blackLightSpotLight;
    [SerializeField]
    private Color[] _color;
    [SerializeField]
    private Renderer _backgroundRenderer;
    [SerializeField]
    private Renderer _backgroundBloodRenderer;
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
    private const string _blackLightTag = "BlackLight", _blackLightInverseTag = "BlackLightInverse";
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


    ////////////Tests
#if DEV
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            changeLightProperty(_phenoLight, _color[1], 24, 4);
            changeLightProperty(_spotLight, _color[1], 57.5f, 5.3f);
        }
    }
#endif

    void Start()
    {
        _originalDirectionalIntensity = _directionalLight.intensity;
        changeLightProperty(_phenoLight, _color[1], 24, 4);
        changeLightProperty(_spotLight, _color[1], 57.5f, 5.3f);
        _ampicillinPulsingLight = GameObject.Find("AmpicillinPulsingLight").GetComponent<PulsingLight>();
        _originMaxPulse = _ampicillinPulsingLight.GetMaxIntensityValue();
        _originMinPulse = _ampicillinPulsingLight.GetMinIntensityValue();
        _hero = Hero.get();
        Color color = _backgroundBloodRenderer.material.color;
        color.a = 0;
        _backgroundBloodRenderer.material.color = color;

        _phenoLight.enabled = true;
        _spotLight.enabled = true;
        _blackLightSpotLight.enabled = false;

        saveCurrentLighting();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == _blackLightTag)
        {
            _blackLight = true;
            _blackLightSpotLight.enabled = true;
            _spotLight.enabled = false;
            _originGradient = col.transform.position;
            _limitGradient = col.transform.GetChild(0).transform.position;
            _ampicillinPulsingLight.TweekRangeIntensity(0, 0.01f);
        }
        if (col.tag == _blackLightInverseTag)
        {
            _blackLight = true;
            _blackLightSpotLight.enabled = true;
            _spotLight.enabled = false;
            _originGradient = col.transform.position;
            _limitGradient = col.transform.GetChild(0).transform.position;
            _ampicillinPulsingLight.TweekRangeIntensity(0, 0.01f);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == _blackLightTag)
        {
            if (this.transform.position.x <= col.transform.position.x)
            {
                _blackLight = false;
                _blackLightSpotLight.enabled = false;
                _spotLight.enabled = true;
                changeLightIntensity(_directionalLight, _originalDirectionalIntensity);
                _ampicillinPulsingLight.TweekRangeIntensity(_originMinPulse, _originMaxPulse);
                startReset();
            }
        }
        if (col.tag == _blackLightInverseTag)
        {
            if (this.transform.position.x >= col.transform.position.x)
            {
                _blackLight = false;
                _blackLightSpotLight.enabled = false;
                _spotLight.enabled = true;
                changeLightIntensity(_directionalLight, _originalDirectionalIntensity);
                _ampicillinPulsingLight.TweekRangeIntensity(_originMinPulse, _originMaxPulse);
                startReset();
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == _blackLightTag)
        {
            float distanceMax = Vector3.Distance(_originGradient, _limitGradient);
            float distance = Vector3.Distance(_originGradient, this.transform.position);
            changeLightIntensity(_directionalLight, 1 - (distance / distanceMax));
            Color color = _backgroundRenderer.material.color;
            color.a = 1 - (distance / distanceMax);
            _backgroundRenderer.material.color = color;
        }
        if (col.tag == _blackLightInverseTag)
        {
            float distanceMax = Vector3.Distance(_originGradient, _limitGradient);
            float distance = Vector3.Distance(_originGradient, this.transform.position);
            changeLightIntensity(_directionalLight, distance / distanceMax);
            Color color = _backgroundRenderer.material.color;
            color.a = distance / distanceMax;
            _backgroundRenderer.material.color = color;
        }
    }

    public void setInjured(float life)
    {
        if (!_blackLight)
        {
            changeLightIntensity(_directionalLight, _lightingSave.GetOriginDirectional() * life);
            changeLightIntensity(_phenoLight, _lightingSave.GetOriginPheno() * life);
            changeLightIntensity(_spotLight, _lightingSave.GetOriginSpot() * life);
        }
        _alphaColor = _backgroundBloodRenderer.material.color;
        _alphaColor.a = 0.5f - life;
        _backgroundBloodRenderer.material.color = _alphaColor;
    }

    public void setDead()
    {
        _alphaColor = _backgroundBloodRenderer.material.color;
        _alphaColor.a = 1f;
        _backgroundBloodRenderer.material.color = _alphaColor;
    }

    public void startReset()
    {
        _lightingSave.GetOriginForReset();
    }

    public void resetLighting(float directional, float pheno, float spotLight)
    {
        _alphaColor = _backgroundBloodRenderer.material.color;
        _alphaColor.a = 0f;
        _backgroundBloodRenderer.material.color = _alphaColor;
        _alphaColor = _backgroundRenderer.material.color;
        _alphaColor.a = 1f;
        _backgroundRenderer.material.color = _alphaColor;
        changeLightIntensity(_directionalLight, directional);
        changeLightIntensity(_phenoLight, pheno);
        changeLightIntensity(_spotLight, spotLight);
    }

    public void saveCurrentLighting()
    {
        _originalDirectionalIntensity = _directionalLight.intensity;
        _originalPhenoLightIntensity = _phenoLight.intensity;
        _originalSpotLightIntensity = _spotLight.intensity;
    }

    public bool isBlackLight()
    {
        return _blackLight;
    }
}
