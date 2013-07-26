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
public class PhenoLight : Phenotype {

  public Light affectedLight;
  public Color color;          //! Color of the Light


  //! Called at the begening
  public override void StartPhenotype()
  {
    affectedLight.color = color;
  }

  /*!
    \brief This function is called as Update in Monobehaviour.
    \details This function is called in the Phenotype class in the Update function
    This function should be implemented and all the graphical action has to be implemented in it.
    \sa Phenotype
   */
  public override void UpdatePhenotype()
  {
    Molecule mol = ReactionEngine.getMoleculeFromName("H2O", _molecules);
    if (mol == null)
      return ;
    float intensity = Phenotype.hill(mol.getConcentration(), 100.0f, 1f, 0f, 8f);
	float colRadius = Phenotype.hill(mol.getConcentration(), 100.0f, 1f, 0f, 7f);
    
	affectedLight.intensity = intensity;
	((SphereCollider)affectedLight.collider).radius = colRadius;
  }
}
