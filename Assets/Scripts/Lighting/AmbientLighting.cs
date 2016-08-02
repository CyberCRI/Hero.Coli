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
    private Material[] _backgroundMaterial;
    private float _originalDirectionalIntensity;
    private PulsingLight _ampicillinPulsingLight;
    private float _originMaxPulse;
    private float _originMinPulse;

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


    public void ChangeBackgroundColor(int colorIndex)
    {
        _background.GetComponent<Renderer>().material = _backgroundMaterial[colorIndex];
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
            ChangeLightIntensity(_directionaleLight, 0f);
            ChangeBackgroundColor(1);
            _ampicillinPulsingLight.TweekRangeIntensity(0, 0.1f);
        }
        
        if (col.tag == "NormalLight")
        {
            ChangeLightIntensity(_directionaleLight, _originalDirectionalIntensity);
            ChangeBackgroundColor(0);
            _ampicillinPulsingLight.TweekRangeIntensity(_originMinPulse, _originMaxPulse);
        }
    }
}
