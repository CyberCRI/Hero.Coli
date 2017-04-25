using UnityEngine;

public class LightingSave : MonoBehaviour
{
    [SerializeField]
    private Light _directionaleLight;
    [SerializeField]
    private Light _phenoLight;
    [SerializeField]
    private Light _spotLight;
    [SerializeField]
    private AmbientLighting _ambLight;

    private float _originalDirectionalIntensity;
    private float _originalPhenoLightIntensity;
    private float _originalSpotLightIntensity;

    // Use this for initialization
    void Start()
    {
        _originalDirectionalIntensity = _directionaleLight.intensity;
        _originalPhenoLightIntensity = _phenoLight.intensity;
        _originalSpotLightIntensity = _spotLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getOriginForReset()
    {
        _ambLight.resetLighting(_originalDirectionalIntensity, _originalPhenoLightIntensity, _originalSpotLightIntensity);
    }

    public void saveCurrentLighting()
    {
        _originalDirectionalIntensity = _directionaleLight.intensity;
        _originalPhenoLightIntensity = _phenoLight.intensity;
        _originalSpotLightIntensity = _spotLight.intensity;
    }

    public float getOriginDirectional()
    {
        return _originalDirectionalIntensity;
    }

    public float getOriginPheno()
    {
        return _originalDirectionalIntensity;
    }

    public float getOriginSpot()
    {
        return _originalDirectionalIntensity;
    }
}
