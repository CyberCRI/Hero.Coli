using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*!
 \brief A phenotype class that represent a light reaction in function of the concentration in Water (H2O)
 in the Midium
 \author Pierre COLLET
 \mail pierre.collet91@gmail.com
 */
public class PhenoToxic : Phenotype {



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
		//the molecule considered toxic is the Y molecule
		Molecule mol = ReactionEngine.getMoleculeFromName ("Y", _molecules);
		if (mol == null)
			return ;
		float intensity = Phenotype.hill (mol.getConcentration(), 50f, 1f, 0.0f, 0.01f);
		gameObject.GetComponent<Hero>().subLife(intensity);
  }

  public void OnTriggerStay(Collider collisionInfo)
  {
//     foreach (ContactPoint contact in collisionInfo.contacts)
//       {
//         Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
//       }
  }
}