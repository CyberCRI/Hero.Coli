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

  public Color         _color;          //! Color of the Light
  public bool isActive;                 //! Activity of the light


  //! Called at the begening
  public override void StartPhenotype()
  {
    gameObject.AddComponent<Light>();
    gameObject.light.color = _color;
    gameObject.light.type = LightType.Point;
    gameObject.light.color = Color.blue;
    gameObject.light.range = 2.5f;
    gameObject.light.intensity = 1;
  }

  /*!
    \brief This function is called as Update in Monobehaviour.
    \details This function is called in the Phenotype class in the Update function
    This function should be implemented and all the graphical action has to be implemented in it.
    \sa Phenotype
   */
  public override void UpdatePhenotype()
  {
    gameObject.light.enabled = true;
    if (!isActive)
      return ;
    Molecule mol = ReactionEngine.getMoleculeFromName("H2O", _molecules);
    if (mol == null)
      return ;
    float intensity = Phenotype.hill(mol.getConcentration(), 100.0f, 1f, 0f, 8f);
    gameObject.light.intensity = intensity;
  }
}
