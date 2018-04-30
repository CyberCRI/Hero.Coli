using UnityEngine;
using System;

/*!
 \brief A phenotype class that represents a light reaction in function of the concentration in Water (H2O)
 in the Medium
 
 
 */
public class PhenoLight : Phenotype
{
    public const string gfpProtein = "FLUO1", rfpProtein = "FLUO2";

    // TODO use a LinkedList to manage overlapping light sources
    public delegate void LightEvent(PhenoLight.LightType type, bool lightOn, bool isGFP);
    public static event LightEvent onLightToggle;
    [SerializeField]
    private Light _phenoLight, _spotLight, _blackLightSpotLight;
    [SerializeField]
    private float _blackLightSpotLightSpotAngle;
    private float _previousIntensity = 0;

    public bool _active = false;
    private string _fluorescenceProtein;
    private bool _isSystemTriggered = false;
    private Molecule _mol = null;
    private TriggeredLightEmitter _triggered;
    private TriggeredLightEmitter _lm;

    public PlayableSound illuminateSound;
    public PlayableSound darkIlluminateSound;

    private const float _maxConcentration = 270f, _maxValue = 8f;
    private const float _steepness = _maxValue / _maxConcentration;

    public enum LightType
    {
        Default,
        Dark,
        None,
    }

    //! Called at the beginning
    public override void StartPhenotype()
    {
        _blackLightSpotLightSpotAngle = _blackLightSpotLight.spotAngle;
        turnLightOff();
    }

    /*!
    \brief This function is called as Update in Monobehaviour.
    \details This function is called in the Phenotype class in the Update function
    This function should be implemented and all the graphical action has to be implemented in it.
    \sa Phenotype
   */
    public override void UpdatePhenotype()
    {
        if (_active)
        {
            if (_mol == null)
            {
                _mol = ReactionEngine.getMoleculeFromName(_fluorescenceProtein, _molecules);
            }

            float intensity = Mathf.Min(_steepness * _mol.getConcentration(), _maxValue);


            if (intensity != _previousIntensity)
            {
                _previousIntensity = intensity;

                _phenoLight.intensity = intensity;
                _spotLight.intensity = intensity;
                _blackLightSpotLight.intensity = intensity;
                _blackLightSpotLight.spotAngle = intensity * _blackLightSpotLightSpotAngle / _maxValue;
            }

            if (null != _triggered)
            {
                if (_mol.getConcentration() > _triggered.threshold)
                {
                    if (!_isSystemTriggered)
                    {
                        turnLightOn();
                    }
                    else
                    {
                        _triggered.triggerStay();
                    }
                }
                // The concentration fell below the threshold and the light must
                // consequently be switched off
                else if (_isSystemTriggered)
                {
                    turnLightOff();
                }
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        // Debug.Log(this.GetType() + " OnTriggerEnter " + col.name);
        // Prepares the future triggering of the light
        manageInactiveCollision(col);
    }

    private void manageInactiveCollision(Collider col)
    {
        if (!_active)
        {
            _lm = col.gameObject.GetComponent<TriggeredLightEmitter>();
            if (null != _lm)
            {
                _triggered = _lm;
                _fluorescenceProtein = _lm.protein;
                _mol = null;
                _active = !String.IsNullOrEmpty(_fluorescenceProtein);
                // Debug.Log(this.GetType() + " manageInactiveCollision _active=" + _active);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        // Debug.Log(this.GetType() + " OnTriggerExit " + col.name);
        _lm = col.gameObject.GetComponent<TriggeredLightEmitter>();
        if (null != _lm)
        {
            turnLightOff();
            _triggered = null;
            _fluorescenceProtein = "";
            _mol = null;
            _active = false;
            // Debug.Log(this.GetType() + " OnTriggerExit _active=" + _active);
        }
    }
    void OnTriggerStay(Collider col)
    {
        manageInactiveCollision(col);
    }

    private void turnLightOff()
    {
        if (_triggered)
        {
            _triggered.triggerExit();
        }
        _isSystemTriggered = false;
        if (_phenoLight)
        {
            if (_spotLight.enabled)
                onLightToggle(LightType.None, true, _fluorescenceProtein == gfpProtein);
            if (_blackLightSpotLight.enabled)
                onLightToggle(LightType.Dark, false, _fluorescenceProtein == gfpProtein);
            onLightToggle(LightType.Default, false, _fluorescenceProtein == gfpProtein);
            _phenoLight.gameObject.SetActive(false);
            _spotLight.gameObject.SetActive(false);
            _blackLightSpotLight.gameObject.SetActive(false);
        }
    }

    private void turnLightOn()
    {
        if (_triggered)
        {
            _triggered.triggerStart();
            _isSystemTriggered = true;

            if (!_spotLight.enabled && _blackLightSpotLight.enabled)
                onLightToggle(LightType.Dark, true, _fluorescenceProtein == gfpProtein);
            else
                onLightToggle(LightType.Default, true, _fluorescenceProtein == gfpProtein);

            _phenoLight.gameObject.SetActive(true);
            _phenoLight.color = _triggered.colorTo;

            _spotLight.gameObject.SetActive(true);
            _spotLight.color = _triggered.colorTo;

            _blackLightSpotLight.gameObject.SetActive(true);
            _blackLightSpotLight.color = _triggered.colorTo;
        }
    }
}
