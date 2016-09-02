using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*!
 \brief A phenotype class that represents the effects of the toxicity of a molecule
 */
public class PhenoToxic : Phenotype {

  public Hero hero;
	public const string ampicillinMoleculeName = "AMPI";

    private Molecule _mol;  
    //TODO extract to config file
    private float _K0 = 0.01f;
    private float _cc0 = 170f;

  //! Called at the beginning
  public override void StartPhenotype()
  {
		initMoleculePhenotype();
  }

	public void initMoleculePhenotype()
	{
		_mol = ReactionEngine.getMoleculeFromName (ampicillinMoleculeName, _molecules);
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
        Logger.Log("_mol == null", Logger.Level.INFO);
        return ;
      }
     		}
		float intensity = _K0*(Mathf.Exp(_mol.getConcentration()/_cc0)-1);
		hero.subLife(intensity);
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