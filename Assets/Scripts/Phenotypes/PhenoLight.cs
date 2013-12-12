using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*!
 \brief A phenotype class that represent a light reaction in function of the concentration in Water (H2O)
 in the Medium
 \author Pierre COLLET
 \mail pierre.collet91@gmail.com
 */
public class PhenoLight : Phenotype {

  	public GameObject phenoLight;   //!< The light that will be affected by the phenotype
  	
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
    	_mol = ReactionEngine.getMoleculeFromName("FLUO1", _molecules);
    	if (_mol == null)
      		return ;
    	float intensity = Phenotype.hill(_mol.getConcentration(), 100.0f, 1f, 0f, 7f);
		float colRadius = Phenotype.hill(_mol.getConcentration(), 100.0f, 1f, 0f, 7f);
    
		phenoLight.light.intensity = intensity;
		if(colliderActivated)
			((SphereCollider)phenoLight.collider).radius = colRadius;
  	}
	
	void OnTriggerEnter(Collider col){
		if(_mol == null)
			return;
		LightEmitter lm = col.gameObject.GetComponent<LightEmitter>();
		if(lm){
			phenoLight.light.enabled = true;
			colliderActivated = true;
			phenoLight.light.color = lm.colorTo;
		}
 	}
	
	void OnTriggerExit(Collider col){
		LightEmitter lm = col.gameObject.GetComponent<LightEmitter>();
		if(lm){
			phenoLight.light.enabled = false;
			((SphereCollider)phenoLight.collider).radius = 0;
			colliderActivated = false;
		}
	}
}
