using UnityEngine;

/*!
 \brief A phenotype class that represents the effects of the toxicity of a molecule
 */
public class PhenoToxic : Phenotype
{
    [SerializeField]
    private Character character;
    public const string ampicillinMoleculeName = "AMPI";
    private Molecule _mol;
    //TODO extract to config file
    private const float _K0 = 0.01f;
    private const float _cc0 = 170f;

    // data points to set curve coefs
    private Vector2 _dataPoint0 = Vector2.zero;
    // set to inflict 1 dps at ampicillin cc = 64.5 (cc when max AmpR is equipped)
    private Vector2 _dataPoint1 = new Vector2(64.5f, 0.01f + Character.lifeRegenerationRate);
    // set to inflict 100 dps at ampicillin cc = 375 (max cc)
    private Vector2 _dataPoint2 = new Vector2(375f, 1f + Character.lifeRegenerationRate);
    // convention: first is slope, second is offset
    private Vector2 _curveCoefs0, _curveCoefs1;

    //! Called at the beginning
    public override void StartPhenotype()
    {
        initMoleculePhenotype();
        initializeCoefs();
    }

    public void initMoleculePhenotype()
    {
        _mol = ReactionEngine.getMoleculeFromName(ampicillinMoleculeName, _molecules);
    }

    private Vector2 getCoefficients(Vector2 v0, Vector2 v1)
    {
        float slope = (v1.y - v0.y) / (v1.x - v0.x);
        float offset = v0.y - slope * v0.x;
        return new Vector2(slope, offset);
    }

    private void initializeCoefs()
    {
        _curveCoefs0 = getCoefficients(_dataPoint0, _dataPoint1);
        _curveCoefs1 = getCoefficients(_dataPoint1, _dataPoint2);
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
        if (_mol == null)
        {
            initMoleculePhenotype();
            if (_mol == null)
            {
                // Debug.Log(this.GetType() + " _mol == null");
                return;
            }
        }

        float concentration = _mol.getConcentration();
        if (0 == concentration)
        {
            return;
        }

        // cost intensive, smooth variant
        // float intensity = _K0 * (Mathf.Exp(_mol.getConcentration() / _cc0) - 1);

        // lighweight, linear
        float intensity = 0;
        if (concentration < _dataPoint1.x)
        {
            // apply linear function defined on domain [_dataPoint0.x, _dataPoint1.x]
            intensity = _curveCoefs0.x * concentration + _curveCoefs0.y;
        }
        else
        {
            // apply linear function defined on domain [_dataPoint1.x, _dataPoint2.x]
            intensity = _curveCoefs1.x * concentration + _curveCoefs1.y;
        }
        Debug.Log(this.GetType() + " computed dps=" + intensity);
        intensity *= Time.deltaTime;
        Debug.Log(this.GetType() + " computed damage=" + intensity);

        character.subLife(intensity);
    }

    public void CancelPhenotype()
    {
        // Debug.Log(this.GetType() + " CancelPhenotype");
        _mol.setConcentration(0f);
    }

    //DEBUG
    /*
    public void OnCollisionEnter(Collision collision) {
      foreach (ContactPoint contact in collision.contacts) {
        // Debug.Log(contact.point);
              // Debug.Log("PhenoToxic contact "+contact);
        Debug.DrawRay(contact.point, new Vector3(contact.normal.x, 0.0f, contact.normal.z) * 10, Color.white, 5.0f);
      }
    }
    */
}