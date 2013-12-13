using UnityEngine;
using System.Collections;

public class PhenoSpeed : Phenotype
{

	//public float minSpeed;
	//public float maxSpeed;
  public float lowSpeed;
  public float medSpeed;
  public float lowCC;
  public float medCC;

	public float addFlagellumThresholdPerc;
	public float removeFlagellumThresholdPerc;
  //public float threshold = 50f;
  //public float steepness = 1f;
	public GameObject additionalFlagellum;
	
	private bool additionalFlagellumOn = false;
	private Molecule _mol = null;

  private float _steepness1;
  private float _steepness2;
  private float _baseSpeed;
	
	//! Called at the beginning
	public override void StartPhenotype ()
	{
		_mol = ReactionEngine.getMoleculeFromName ("MOV", _molecules);
	}

  public float getIntensity(float cc)
  {
    if(cc < lowCC)
    {
      return _baseSpeed + cc*_steepness1;
    }
    else
    {
      return lowSpeed + (cc - lowCC)*_steepness2;
    }
  }

  public void setBaseSpeed(float speed)
  {
    _baseSpeed = speed;
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

		//float intensity = Phenotype.hill (_mol.getConcentration(), threshold, steepness, minSpeed, maxSpeed);
    if(_steepness1 == 0)
    {
      _steepness1 = (lowSpeed - _baseSpeed)/lowCC;
    }
    if(_steepness2 == 0)
    {
      _steepness2 = (medSpeed - lowSpeed)/(medCC - lowCC);
    }
    float intensity = getIntensity(_mol.getConcentration());
		gameObject.GetComponent<CellControl>().currentMoveSpeed = intensity;

    Logger.Log("PhenoSpeed intensity="+intensity
      +"\n_base="+_baseSpeed
      +"\n_steepness1="+_steepness1
      +"\n_steepness2="+_steepness2
      , Logger.Level.ONSCREEN);

		
		if (!additionalFlagellumOn && (intensity > addFlagellumThresholdPerc*medSpeed))
		{
			AddFlagellum();
		} else if (additionalFlagellumOn && (intensity < removeFlagellumThresholdPerc*medSpeed))
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
