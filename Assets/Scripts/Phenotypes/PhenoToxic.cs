using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*!
 \brief A phenotype class that represents the effects of the toxicity of a molecule
 \author Raphael GOUJET
 \mail raphael.goujet@gmail.com
 */
public class PhenoToxic : Phenotype {

  private Molecule _mol;
  public Hero hero;

  //! Called at the beginning
  public override void StartPhenotype()
  {
	_mol = ReactionEngine.getMoleculeFromName ("AMPI", _molecules);
  }

  /*!
    \brief This function is called as Update in Monobehaviour.
    \details This function is called in the Phenotype class in the Update function
    This function should be implemented and all the graphical action has to be implemented in it.
    \sa Phenotype
   */
  public override void UpdatePhenotype()
  {
		//the molecule considered toxic is the "AMPI" molecule
		if(_mol == null)
		{
		  _mol = ReactionEngine.getMoleculeFromName ("AMPI", _molecules);
		  if (_mol == null)
			return ;
		}
    float K0 = 0.01f;
    float cc0 = 170f;
		float intensity = K0*(Mathf.Exp(_mol.getConcentration()/cc0)-1);
		hero.subLife(intensity);
  }
	
  //DEBUG
  /*
  public void OnCollisionEnter(Collision collision) {
    foreach (ContactPoint contact in collision.contacts) {
      //Debug.Log(contact.point);
			Debug.Log ("contact "+contact);
	  Debug.DrawRay(contact.point, new Vector3(contact.normal.x, 0.0f, contact.normal.z) * 10, Color.white, 5.0f);
	}
  }
  */
}