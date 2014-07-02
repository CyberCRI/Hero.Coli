using UnityEngine;
using System;
using System.Collections;

// ========================== DEGRADATION ================================

/*!
  \brief This class manages degradation reactions
  \details
  Manage the degradation of a specific molecule inside the cell or in a specific medium.
  In this simulation, the degradation is determined by the degradation constant (half-life) of a specific chemical or protein.
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
  protected override bool CharacEquals(IReaction reaction)
  {
    Degradation degradation = reaction as Degradation;
    return (degradation != null)
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
      mol.subConcentration(mol.getDegradationRate() * mol.getConcentration() * _reactionSpeed * ReactionEngine.reactionsSpeed);
    else
      mol.subNewConcentration(delta * _reactionSpeed * ReactionEngine.reactionsSpeed);
  }
}