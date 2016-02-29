using UnityEngine;
using System;

/*!
 \brief A phenotype class that represents a light reaction in function of the concentration in Water (H2O)
 in the Medium
 
 
 */
public class PhenoLight : Phenotype {

  // TODO use a LinkedList to manage overlapping light sources

    public Light phenoLight;   //!< The light that will be affected by the phenotype
    
    private string fluorescenceProtein;
    private bool isSystemTriggered = false;
    private Molecule _mol = null;
    private TriggeredLightEmitter _triggered;

  //! Called at the beginning
  public override void StartPhenotype()
  {
    //affectedLight.color = color;
  }

  	/*!
    \brief This function is called as Update in Monobehaviour.
    \details This function is called in the Phenotype class in the Update function
    This function should be implemented and all the graphical action has to be implemented in it.
    \sa Phenotype
   */
    public override void UpdatePhenotype()
    {
        if(!String.IsNullOrEmpty(fluorescenceProtein))
        {
            _mol = ReactionEngine.getMoleculeFromName(fluorescenceProtein, _molecules);
            if (_mol == null) {
                return;   
            }
                
            float intensity = Phenotype.hill(_mol.getConcentration(), 100.0f, 1f, 0f, 7f);
            float colRadius = Phenotype.hill(_mol.getConcentration(), 100.0f, 1f, 0f, 7f);
    
            phenoLight.intensity = intensity;
            
            if(null != _triggered)
            {
                if(_mol.getConcentration() > _triggered.threshold)
                {
                    if(!isSystemTriggered)
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
                else if (isSystemTriggered)
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
            fluorescenceProtein = lm.protein;
		}
 	}
     
	void OnTriggerExit(Collider col)
    {
		TriggeredLightEmitter lm = col.gameObject.GetComponent<TriggeredLightEmitter>();
		if(null != lm){
            turnLightOff();
            _triggered = null;
            fluorescenceProtein = "";
		}
	}
    
    private void turnLightOff()
    {
        if(_triggered)
        {
            _triggered.triggerExit();
        }
        isSystemTriggered = false;
        if(phenoLight)
        {
            phenoLight.enabled = false;   
        }
    }
    
    private void turnLightOn()
    {
        if(_triggered)
        {
            _triggered.triggerStart();
            isSystemTriggered = true;
            if(phenoLight)
            {
                phenoLight.enabled = true;
                phenoLight.color = _triggered.colorTo;
            }
        }
    }
}
