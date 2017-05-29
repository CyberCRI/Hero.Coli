using UnityEngine;
using System;

/*!
 \brief A phenotype class that represents a light reaction in function of the concentration in Water (H2O)
 in the Medium
 
 
 */
public class PhenoLight : Phenotype
{

    // TODO use a LinkedList to manage overlapping light sources

    [SerializeField]
    private Light _phenoLight, _spotLight, _blackLightSpotLight;

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

    //! Called at the beginning
    public override void StartPhenotype()
    {
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

            _phenoLight.intensity = intensity;
            _spotLight.intensity = intensity;
            _blackLightSpotLight.intensity = intensity;

            if (null != _triggered)
            {
                if (_mol.getConcentration() > _triggered.threshold)
                {
                    if (!_isSystemTriggered)
                    {
						if (_blackLightSpotLight.enabled)
							darkIlluminateSound.PlayIfNotPlayed ();
						else
							illuminateSound.PlayIfNotPlayed ();
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

            _phenoLight.gameObject.SetActive(true);
            _phenoLight.color = _triggered.colorTo;

            _spotLight.gameObject.SetActive(true);
            _spotLight.color = _triggered.colorTo;

            _blackLightSpotLight.gameObject.SetActive(true);
            _blackLightSpotLight.color = _triggered.colorTo;
        }
    }
}
