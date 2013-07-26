using UnityEngine;
using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief This class represent an active transport between two mediums.
  \details This reaction is the same reaction as an EnzymeReaction but the Substrate
  is consumed in a Medium and release in another one.
  \sa EnzymeReaction
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
public class ActiveTransportReaction : EnzymeReaction
{
  private Medium _srcMedium;            //!< The source Medium (Where substrate will be consumed)
  private Medium _dstMedium;            //!< The destination Medium (Where product will be released)

  public Medium getSrcMedium() { return _srcMedium; }
  public Medium getDstMedium() { return _dstMedium; }
  public void setSrcMedium(Medium med) { _srcMedium = med; }
  public void setDstMedium(Medium med) { _dstMedium = med; }


  //! Default Constructor
  public ActiveTransportReaction()
  {
  }

  //! Copy Constructor
  public ActiveTransportReaction(ActiveTransportReaction r) : base(r)
  {
    _srcMedium = r._srcMedium;
    _dstMedium = r._dstMedium;
  }

  /*
    \brief This function do the reaction.
    \details The ActiveTransportReaction is a reaction that is exactly the same as EnzymeReaction.
    The only difference is that the production are added to the destination medium and not to the 
    medium where the reaction is going on.
    \param molecules dont usefull, can be set to null
    \sa execEnzymeReaction
   */
  public override void react(ArrayList molecules)
  {
    if (!_isActive)
      return;

    ArrayList molSrcMed = _srcMedium.getMolecules();
    ArrayList molDstMed = _dstMedium.getMolecules();
    Molecule substrate = ReactionEngine.getMoleculeFromName(_substrate, molSrcMed);
    if (substrate == null)
      return ;

    float delta = execEnzymeReaction(molSrcMed) * _reactionSpeed * ReactionEngine.reactionsSpeed;

    float energyCoef;
    float energyCostTot;
    if (delta > 0f && _energyCost > 0f && enableEnergy)
      {
        energyCostTot = _energyCost * delta;
        energyCoef = _medium.getEnergy() / energyCostTot;
        if (energyCoef > 1f)
          energyCoef = 1f;
        _medium.subEnergy(energyCostTot);
      }
    else
      energyCoef = 1f;

    delta *= energyCoef;
    if (enableSequential)
      substrate.subConcentration(delta);
    else
      substrate.subNewConcentration(delta);
    foreach (Product pro in _products)
      {
        Molecule mol = ReactionEngine.getMoleculeFromName(pro.getName(), molDstMed);
        if (enableSequential)
          mol.addConcentration(delta);
        else
          mol.addNewConcentration(delta);
      }    
  }
}
