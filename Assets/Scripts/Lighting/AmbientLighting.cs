// #define DEV
using UnityEngine;

public class AmbientLighting : MonoBehaviour
{
    #region Events
    public delegate void LightEvent(bool lightOn);
	public static event LightEvent onLightToggle;
    #endregion

    [SerializeField]
	private float _snapshotTransitionTime;

	private bool _lowLight;

    [SerializeField]
    private Light _directionalLight;

    [SerializeField]
    private Light _phenoLight, _spotLight, _blackLightSpotLight;

    [SerializeField]
    private Color[] _color;

    [SerializeField]
    private Renderer _backgroundRenderer;

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

    private Vector3 entryPoint;
    private Vector3 exitPoint;

    private float entryIntensity = 1f;
    private float exitIntensity = 0.1f;

    private float entryRatio = 0f;
    private float exitRatio = 0f;

    private Character _character;
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

        entryIntensity = _originalDirectionalIntensity;

        changeLightProperty(_phenoLight, _color[1], 24, 4);
        changeLightProperty(_spotLight, _color[1], 57.5f, 5.3f);

        _ampicillinPulsingLight = GameObject.Find("AmpicillinPulsingLight").GetComponent<PulsingLight>();

        _originMaxPulse = _ampicillinPulsingLight.GetMaxIntensityValue();
        _originMinPulse = _ampicillinPulsingLight.GetMinIntensityValue();

        _character = Character.get();

		_backgroundBloodRenderer = GameObject.FindGameObjectWithTag ("BackgroundBlood").GetComponent<Renderer> ();
		Color color = _backgroundBloodRenderer.material.color;
		color.a = 0;
        _backgroundBloodRenderer.material.color = color;

		onLightToggle (true);

        _phenoLight.enabled = true;
        _spotLight.enabled = true;
        _blackLightSpotLight.enabled = false;

        saveCurrentLighting();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag(_blackLightTag) || col.CompareTag(_blackLightInverseTag))
        {
            _blackLight = true;
            _blackLightSpotLight.enabled = true;
            _spotLight.enabled = false;
            _originGradient = col.transform.position;

            entryPoint = col.transform.Find("Entry").transform.position;
            exitPoint = col.transform.Find("Exit").transform.position;

            _ampicillinPulsingLight.TweekRangeIntensity(0, 0.01f);
        }
    }

    void OnTriggerExit(Collider col)
    {
        // BESOIN DE TESTER SI SORTIE PAR ENTREE OU SORTIE

        //if (col.CompareTag(_blackLightTag) || col.CompareTag(_blackLightInverseTag))
        //{
        //    _blackLight = false;
        //    _blackLightSpotLight.enabled = false;
        //    _spotLight.enabled = true;
        //    changeLightIntensity(_directionalLight, _originalDirectionalIntensity);
        //    _ampicillinPulsingLight.TweekRangeIntensity(_originMinPulse, _originMaxPulse);
        //    startReset();
        //}
    }

    void OnTriggerStay(Collider col)
    {
        bool isBlackLight = col.CompareTag(_blackLightTag);
        bool isBlackLightInverse = col.CompareTag(_blackLightInverseTag);

        if (isBlackLight || isBlackLightInverse)
        {
            Vector3 entryToPlayerVector = transform.position - entryPoint;

            float entryExitDistance = Vector3.Distance(entryPoint, exitPoint);

            Debug.Log("EnEx" + entryExitDistance);

            Vector3 entryToExitVector = (exitPoint - entryPoint).normalized;

            Vector3 PlayerPosInExitEnterLine = Vector3.Project(entryToPlayerVector, entryToExitVector);

            float entryPlayerDistance = PlayerPosInExitEnterLine.magnitude;
            Debug.Log("EnP' " + entryPlayerDistance);
            exitRatio = entryPlayerDistance / entryExitDistance;
            entryRatio = 1 - exitRatio;

            float lightIntensity = entryRatio * entryIntensity + exitRatio * exitIntensity;
            changeLightIntensity(_directionalLight, lightIntensity);

            Color color = _backgroundRenderer.material.color;
            color.a = lightIntensity;

            if (isBlackLight && color.a <= 0.25f && !_lowLight)
            {
                onLightToggle(false);
                _lowLight = true;
            }
            else if (isBlackLightInverse && color.a >= 0.75f && _lowLight)
            {
                onLightToggle(true);
                _lowLight = false;
            }
            _backgroundRenderer.material.color = color;
        }
    }

    // life is between 0f and 1f
    public void setInjured(float life)
    {
#if DEV
        if (BackendManager.isHurtLightEffectsOn && !_blackLight)
        {
            changeLightIntensity(_directionalLight, _lightingSave.getOriginDirectional() * life);
            changeLightIntensity(_phenoLight, _lightingSave.getOriginPheno() * life);
            changeLightIntensity(_spotLight, _lightingSave.getOriginSpot() * life);
        }
#endif
        _alphaColor = _backgroundBloodRenderer.material.color;
        _alphaColor.a = 1f - life * life;
        _backgroundBloodRenderer.material.color = _alphaColor;
    }

    public void setDead()
    {
        _alphaColor = _backgroundBloodRenderer.material.color;
        _alphaColor.a = 1f;
        _backgroundBloodRenderer.material.color = _alphaColor;
		onLightToggle (true);
		_lowLight = false;
    }

    public void startReset()
    {
        _lightingSave.getOriginForReset();
    }

    public void resetLighting(float directional, float pheno, float spotLight)
    {
        _alphaColor = _backgroundBloodRenderer.material.color;
        _alphaColor.a = 0f;
        _backgroundBloodRenderer.material.color = _alphaColor;
        _alphaColor = _backgroundRenderer.material.color;
        _alphaColor.a = 1f;
        _backgroundRenderer.material.color = _alphaColor;
		onLightToggle (true);
		_lowLight = false;
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
