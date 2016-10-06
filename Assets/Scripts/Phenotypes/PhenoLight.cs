using UnityEngine;
using System;

/*!
 \brief A phenotype class that represents a light reaction in function of the concentration in Water (H2O)
 in the Medium
 
 
 */
public class PhenoLight : Phenotype {

  // TODO use a LinkedList to manage overlapping light sources

    [SerializeField]
    private Light _phenoLight, _spotLight, _blackLightSpotLight;
    
    private bool _active = false;
    private string _fluorescenceProtein;
    private bool _isSystemTriggered = false;
    private Molecule _mol = null;
    private TriggeredLightEmitter _triggered;

    private const float _maxConcentration = 270f, _maxValue = 8f;
    private const float _steepness = _maxValue / _maxConcentration;

  //! Called at the beginning
  public override void StartPhenotype()
  {
      
  }

  	/*!
    \brief This function is called as Update in Monobehaviour.
    \details This function is called in the Phenotype class in the Update function
    This function should be implemented and all the graphical action has to be implemented in it.
    \sa Phenotype
   */
    public override void UpdatePhenotype()
    {
        if(_active)
        {
            if (_mol == null) {
                _mol = ReactionEngine.getMoleculeFromName(_fluorescenceProtein, _molecules);   
            }
                
            float intensity = Mathf.Min(_steepness * _mol.getConcentration(), _maxValue);
    
            _phenoLight.intensity = intensity;
            _spotLight.intensity = intensity;
            _blackLightSpotLight.intensity = intensity;
            
            if(null != _triggered)
            {
                if(_mol.getConcentration() > _triggered.threshold)
                {
                    if(!_isSystemTriggered)
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
        // Prepares the future triggering of the light
		TriggeredLightEmitter lm = col.gameObject.GetComponent<TriggeredLightEmitter>();
		if(null != lm)
        {
            _triggered = lm;
            _fluorescenceProtein = lm.protein;
            _active = !String.IsNullOrEmpty(_fluorescenceProtein);
		}
 	}
     
	void OnTriggerExit(Collider col)
    {
		TriggeredLightEmitter lm = col.gameObject.GetComponent<TriggeredLightEmitter>();
		if(null != lm){
            turnLightOff();
            _triggered = null;
            _fluorescenceProtein = "";
            _active = false;
		}
	}
    
    private void turnLightOff()
    {
        if(_triggered)
        {
            _triggered.triggerExit();
        }
        _isSystemTriggered = false;
        if(_phenoLight)
        {
            _phenoLight.gameObject.SetActive(false);
            _spotLight.gameObject.SetActive(false);
            _blackLightSpotLight.gameObject.SetActive(false);
        }
    }
    
    private void turnLightOn()
    {
        if(_triggered)
        {
            _triggered.triggerStart();
            _isSystemTriggered = true;
            
            _phenoLight.gameObject.SetActive(true);
            _phenoLight.color = _triggered.colorTo;

            _spotLight.gameObject.SetActive(true);
            _spotLight.color = _triggered.colorTo;

            _blackLightSpotLight.gameObject.SetActive(true);
            _spotLight.color = _triggered.colorTo;
        }
    }
}
