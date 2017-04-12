using UnityEngine;
using System.Collections;

// ========================== DEGRADATION ================================

/*!
  \brief This class manages degradation reactions
  \details
  Manages the degradation of a specific molecule inside the cell or in a specific medium.
  In this simulation, the degradation is determined by the degradation constant (half-life) of a specific chemical or protein.
  Loading done through medium initialization by browsing the list of molecules, which have a degradation rate attribute.
 */
public class Degradation : IReaction
{
  private float _degradationRate;               //! The degradation speed
  private string _molName;                      //! Molecule name
  
  //! Default constructor
  public Degradation(float degradationRate, string molName)
  {
    _degradationRate = degradationRate;
    _molName = molName;
  }
  
  //! Copy Constructor
  public Degradation(Degradation r) : base(r)
  {
    _degradationRate = r._degradationRate;
    _molName = r._molName;
  }
  
  /* !
    \brief Checks that two reactions have the same Degradation field values.
    \param reaction The reaction that will be compared to 'this'.
   */
  protected override bool PartialEquals(IReaction reaction)
  {
    Degradation degradation = reaction as Degradation;
    return (degradation != null)
      && base.PartialEquals(reaction)
      && (_degradationRate == degradation._degradationRate)
      && (_molName == degradation._molName);
  }
  
  /*!
    \details The degradation reaction following the formula above:

                [X] = degradationRate * [X]

    \param molecules The list of molecules
   */
  public override void react(ArrayList molecules)
  {
    if (!_isActive)
      return;
    
    Molecule mol = ReactionEngine.getMoleculeFromName(_molName, molecules);
    float delta = mol.getDegradationRate() * mol.getConcentration();
    if (enableSequential)
      mol.subConcentration(mol.getDegradationRate() * mol.getConcentration() * _reactionSpeed * ReactionEngine.reactionSpeed * Time.deltaTime);
    else
      mol.subNewConcentration(delta * _reactionSpeed * ReactionEngine.reactionSpeed * Time.deltaTime);
  }

    public override bool hasValidData()
    {
        bool valid = base.hasValidData()
            && !string.IsNullOrEmpty(_molName);
        if(valid)
        {
          if(0 == _degradationRate)
          {
            Debug.LogWarning(this.GetType() + " hasValidData please check that you really intended a degradation rate of 0 for molecule "+_molName);
          }
        }
        return valid;
    }
}