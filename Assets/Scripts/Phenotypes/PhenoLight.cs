using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*!
 \brief A phenotype class that represents a light reaction in function of the concentration in Water (H2O)
 in the Medium
 
 
 */
public class PhenoLight : Phenotype {

  // TODO use a LinkedList to manage overlapping light sources

  public GameObject phenoLight;   //!< The light that will be affected by the phenotype
  private string fluorescenceProtein;
  	
	private bool colliderActivated = false;
	private Molecule _mol = null;

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
      if (_mol == null)
        return ;
      float intensity = Phenotype.hill(_mol.getConcentration(), 100.0f, 1f, 0f, 7f);
      float colRadius = Phenotype.hill(_mol.getConcentration(), 100.0f, 1f, 0f, 7f);
    
      phenoLight.light.intensity = intensity;
      if(colliderActivated)
        ((SphereCollider)phenoLight.collider).radius = colRadius;
    }
  }
	
	void OnTriggerEnter(Collider col){
		//if(_mol == null)
			//return;
		LightEmitter lm = col.gameObject.GetComponent<LightEmitter>();
		if(null != lm && col.gameObject != null){
			phenoLight.light.enabled = true;
			colliderActivated = true;
			phenoLight.light.color = lm.colorTo;
      fluorescenceProtein = lm.protein;
		}
 	}
	
	void OnTriggerExit(Collider col){
		LightEmitter lm = col.gameObject.GetComponent<LightEmitter>();
		if(null != lm && col.gameObject != null){
			phenoLight.light.enabled = false;
			((SphereCollider)phenoLight.collider).radius = 0;
			colliderActivated = false;
      fluorescenceProtein = "";
		}
	}
}
