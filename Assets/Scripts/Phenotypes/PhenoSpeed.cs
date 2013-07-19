using UnityEngine;
using System.Collections;

public class PhenoSpeed : Phenotype
{

	public float minSpeed = 15f;
	public float maxSpeed = 45f;
	
	
	//! Called at the begening
	public override void StartPhenotype ()
	{
		
	}

	/*!
    \brief This function is called as Update in Monobehaviour.
    \details This function is called in the Phenotype class in the Update function
    This function should be implemented and all the graphical action has to be implemented in it.
    \sa Phenotype
   */
	public override void UpdatePhenotype ()
	{
		Molecule mol = ReactionEngine.getMoleculeFromName ("H2O", _molecules);
		if (mol == null)
			return ;
		float intensity = Phenotype.hill (mol.getConcentration(), 50f, 1f, minSpeed, maxSpeed);
		gameObject.GetComponent<CellControl>().moveSpeed = intensity;
	}
}
