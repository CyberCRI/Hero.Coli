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
	private string _toxicName = "AMPI";

  //! Called at the beginning
  public override void StartPhenotype()
  {
		initMoleculePhenotype();
  }

	public void initMoleculePhenotype()
	{
		_mol = ReactionEngine.getMoleculeFromName (_toxicName, _molecules);
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
			initMoleculePhenotype();
		  if (_mol == null)
      {
        Logger.Log("_mol == null", Logger.Level.ONSCREEN);
        return ;
      }
		}
    float K0 = 0.01f;
    float cc0 = 170f;
		float intensity = K0*(Mathf.Exp(_mol.getConcentration()/cc0)-1);
		hero.subLife(intensity);
    Logger.Log("toxic life -= "+intensity, Logger.Level.ONSCREEN);
  }



  public void CancelPhenotype() {
    Logger.Log("PhenoToxic::CancelPhenotype", Logger.Level.WARN);
    _mol.setConcentration(0f);
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