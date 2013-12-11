using UnityEngine;
using System.Collections;

public class PhenoSpeed : Phenotype
{

	public float minSpeed;
	public float maxSpeed;
	public float addFlagellumThresholdPerc;
	public float removeFlagellumThresholdPerc;
	public GameObject additionalFlagellum;
	
	private bool additionalFlagellumOn = false;
	private Molecule _mol = null;	
	
	//! Called at the beginning
	public override void StartPhenotype ()
	{
		_mol = ReactionEngine.getMoleculeFromName ("MOV", _molecules);
	}

	/*!
    \brief This function is called as Update in Monobehaviour.
    \details This function is called in the Phenotype class in the Update function
    This function should be implemented and all the graphical action has to be implemented in it.
    \sa Phenotype
   */
	public override void UpdatePhenotype ()
	{
		if (_mol == null)
		{
			_mol = ReactionEngine.getMoleculeFromName ("MOV", _molecules);
			if (_mol == null)
				return ;
		}
		float intensity = Phenotype.hill (_mol.getConcentration(), 50f, 1f, minSpeed, maxSpeed);
		gameObject.GetComponent<CellControl>().moveSpeed = intensity;
		
		if (!additionalFlagellumOn && (intensity > addFlagellumThresholdPerc*maxSpeed))
		{
			AddFlagellum();
		} else if (additionalFlagellumOn && (intensity < removeFlagellumThresholdPerc*maxSpeed))
		{
			RemoveFlagellum();
		}
	}
	
	private void AddFlagellum()
	{
		Logger.Log("PhenoSpeed::AddFlagellum add flagellum", Logger.Level.INFO);
		if (additionalFlagellum != null)
		{
			additionalFlagellum.SetActive(true);
		}
		
		additionalFlagellumOn = true;
	}
	
	private void RemoveFlagellum()
	{
		Logger.Log("PhenoSpeed::RemoveFlagellum", Logger.Level.INFO);
		if(additionalFlagellum != null)
		{
			additionalFlagellum.SetActive(false);
		}
		additionalFlagellumOn = false;
	}
}
