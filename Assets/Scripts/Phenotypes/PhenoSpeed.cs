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

  /*
  public float add2ndFlagellumThresholdPerc;
  public float rem2ndFlagellumThresholdPerc;
  public float add3rdFlagellumThresholdPerc;
  public float rem3rdFlagellumThresholdPerc;
  */

  public float add2ndFlagellumThreshold;
  public float rem2ndFlagellumThreshold;
  public float add3rdFlagellumThreshold;
  public float rem3rdFlagellumThreshold;


  //public float threshold = 50f;
  //public float steepness = 1f;
	public GameObject centralFlagellum;
  public GameObject leftFlagellum;
  public GameObject rightFlagellum;
	
	private int flagellaCount = 1;
	private Molecule _mol = null;
	private string _speedName = "MOV";

  private float _steepness1;
  private float _steepness2;
  private float _baseSpeed;
	
	//! Called at the beginning
	public override void StartPhenotype ()
	{
    gameObject.GetComponent<SwimAnimator>().safeInitAnims();
    set1Flagella();
		initMoleculePhenotype();
	}

	public void initMoleculePhenotype()
	{
		_mol = ReactionEngine.getMoleculeFromName (_speedName, _molecules);
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

  private void updateFlagellaCount(float speed)
  {
    switch(flagellaCount)
    {
      case 1:
        if(speed > add2ndFlagellumThreshold)
          //if(speed > add2ndFlagellumThresholdPerc*lowSpeed)
          set2Flagella();
        break;
      case 2:
        if(speed > add3rdFlagellumThreshold)
          //if(speed > add3rdFlagellumThresholdPerc*medSpeed)
          set3Flagella();
        else if(speed < rem2ndFlagellumThreshold)
          //else if(speed < rem2ndFlagellumThresholdPerc*lowSpeed)
          set1Flagella();
        break;
      case 3:
        if(speed < rem3rdFlagellumThreshold)
          //if(speed < rem3rdFlagellumThresholdPerc*medSpeed)
          set2Flagella();
        break;
      default:
        Logger.Log("PhenoSpeed::updateFlagellaCount bad flagellaCount="+flagellaCount, Logger.Level.WARN);
        break;
    }
  }

  private void set1Flagella()
  {
    flagellaCount = 1;
    leftFlagellum.SetActive(false);
    centralFlagellum.SetActive(true);
    rightFlagellum.SetActive(false);
  }

  private void set2Flagella()
  {
    flagellaCount = 2;
    leftFlagellum.SetActive(true);
    centralFlagellum.SetActive(false);
    rightFlagellum.SetActive(true);
  }

  private void set3Flagella()
  {
    flagellaCount = 3;
    leftFlagellum.SetActive(true);
    centralFlagellum.SetActive(true);
    rightFlagellum.SetActive(true);
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
			initMoleculePhenotype();
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

    /*
    Logger.Log("PhenoSpeed intensity="+intensity
      //+"\n_base="+_baseSpeed
      //+"\n_steepness1="+_steepness1
      //+"\n_steepness2="+_steepness2
      , Logger.Level.ONSCREEN);
      */

		
		updateFlagellaCount(intensity);
	}
}
