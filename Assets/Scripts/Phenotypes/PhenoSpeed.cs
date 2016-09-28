using UnityEngine;

public class PhenoSpeed : Phenotype
{
    [SerializeField]
    private FlagellaSetter _flagellaSetter;

    public bool devMode = false;

    //public float minSpeed;
    //public float maxSpeed;
    public float baseSpeed;
    public float lowSpeed;
    public float medSpeed;
    public float zeroCC;
    public float lowCC;
    public float medCC;

    /*
    public float add2ndFlagellumThresholdPerc;
    public float rem2ndFlagellumThresholdPerc;
    public float add3rdFlagellumThresholdPerc;
    public float rem3rdFlagellumThresholdPerc;
    */

    public float add1stFlagellumThreshold;
    public float rem1stFlagellumThreshold;
    public float add2ndFlagellumThreshold;
    public float rem2ndFlagellumThreshold;
    public float add3rdFlagellumThreshold;
    public float rem3rdFlagellumThreshold;
    public float add4thFlagellumThreshold;
    public float rem4thFlagellumThreshold;
    
    private Molecule _mol = null;
    private const string _speedName = "MOV";

    private float _steepness0;
    private float _steepness1;
    private float _steepness2;
  
    private float intensity;
    private CellControl cellControl;
	
    private int _defaultFlagellaCount = 0;
    public void setDefaultFlagellaCount(int defaultCount)
    {
        _defaultFlagellaCount = ((defaultCount>1)||(defaultCount<0))?1:defaultCount;
    }
    
	//! Called at the beginning
	public override void StartPhenotype ()
	{
        gameObject.GetComponent<SwimAnimator>().safeInitAnims();
    _flagellaSetter.setFlagellaCount(1);
		initMoleculePhenotype();
	}

	public void initMoleculePhenotype()
	{
		_mol = ReactionEngine.getMoleculeFromName (_speedName, _molecules);
	}

  public float getIntensity(float cc)
  {
        if((0 == _defaultFlagellaCount) && (cc < zeroCC))
        {
            return cc*_steepness0;
        }
        else if(cc < lowCC)
        {
        return baseSpeed + cc*_steepness1;
        }
        else
        {
        return lowSpeed + (cc - lowCC)*_steepness2;
        }
  }

  public void setBaseSpeed(float speed)
  {
    baseSpeed = speed;
  }

    private void updateFlagellaCount(float speed)
    {
        switch (_flagellaSetter.flagellaCount)
        {
            case 0:
                if (speed > add1stFlagellumThreshold)
                    _flagellaSetter.setFlagellaCount(1);
                break;
            case 1:
                if (speed > add2ndFlagellumThreshold)
                    _flagellaSetter.setFlagellaCount(2);
                else if (speed < rem1stFlagellumThreshold)
                    _flagellaSetter.setFlagellaCount(0);
                break;
            case 2:
                if (speed > add3rdFlagellumThreshold)
                    _flagellaSetter.setFlagellaCount(3);
                else if (speed < rem2ndFlagellumThreshold)
                    _flagellaSetter.setFlagellaCount(1);
                break;
            case 3:
                if (speed > add4thFlagellumThreshold)
                    _flagellaSetter.setFlagellaCount(4);
                else if (speed < rem3rdFlagellumThreshold)
                    _flagellaSetter.setFlagellaCount(2);
                break;
            case 4:
                if (speed < rem4thFlagellumThreshold)
                    _flagellaSetter.setFlagellaCount(3);
                break;
            default:
                Debug.LogWarning("PhenoSpeed::updateFlagellaCount bad flagellaCount=" + _flagellaSetter.flagellaCount);
                break;
        }
          
    }

  public int getFlagellaCount()
  {
    return _flagellaSetter.flagellaCount;
  }

	/*!
    \brief This function is called as Update in Monobehaviour.
    \details This function is called in the Phenotype class in the Update function
    This function should be implemented and all the graphical action has to be implemented in it.
    \sa Phenotype
   */
	public override void UpdatePhenotype ()
	{
        if(devMode)
        {
            initialize();
        }
        
		if (_mol == null)
		{
			initMoleculePhenotype();
			if (_mol == null)
				return ;
		}

		//float intensity = Phenotype.hill (_mol.getConcentration(), threshold, steepness, minSpeed, maxSpeed);
    
    intensity = getIntensity(_mol.getConcentration());
    
		cellControl.currentMoveSpeed = intensity;

    /*
    Logger.Log("PhenoSpeed intensity="+intensity
      //+"\n_base="+baseSpeed
      //+"\n_steepness1="+_steepness1
      //+"\n_steepness2="+_steepness2
      , Logger.Level.ONSCREEN);
      */

		
		updateFlagellaCount(intensity);
	}
    
    public override void initialize()
    {
        base.initialize();
        
        _flagellaSetter.setFlagellaCount(0);
        if(null == cellControl)
        {
            cellControl = gameObject.GetComponent<CellControl>(); 
        }
    
        if(_steepness1 == 0)
        {
            _steepness1 = (lowSpeed - baseSpeed)/lowCC;
        }
        if(_steepness2 == 0)
        {
            _steepness2 = (medSpeed - lowSpeed)/(medCC - lowCC);
        }
        if(_steepness0 == 0)
        {
            _steepness0 = (baseSpeed + lowCC*_steepness1)/lowCC;
        }
    }
}
