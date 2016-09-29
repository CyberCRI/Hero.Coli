using UnityEngine;

public class LightingSave : MonoBehaviour {


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
    void Start () {
        _originalDirectionalIntensity = _directionaleLight.intensity;
        _originalPhenoLightIntensity = _phenoLight.intensity;
        _originalSpotLightIntensity = _spotLight.intensity;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GetOriginForReset()
    {
        _ambLight.resetLighting(_originalDirectionalIntensity,_originalPhenoLightIntensity,_originalSpotLightIntensity);
    }

    public void SaveCurrentLighting()
    {
        _originalDirectionalIntensity = _directionaleLight.intensity;
        _originalPhenoLightIntensity = _phenoLight.intensity;
        _originalSpotLightIntensity = _spotLight.intensity;
    }

    public float GetOriginDirectional()
    {
        return _originalDirectionalIntensity;
    }

    public float GetOriginPheno()
    {
        return _originalDirectionalIntensity;
    }

    public float GetOriginSpot()
    {
        return _originalDirectionalIntensity;
    }
}
