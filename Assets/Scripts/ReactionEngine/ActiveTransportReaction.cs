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

  /*
    \brief This function do the reaction.
    \param molecules dont usefull, must be set to null
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

    float delta = execEnzymeReaction(molSrcMed) * _reactionSpeed * ReactionEngine.reactionsSpeed * Time.deltaTime;

    substrate.setConcentration(substrate.getConcentration() - delta);
    foreach (Product pro in _products)
      {
        Molecule mol = ReactionEngine.getMoleculeFromName(pro.getName(), molDstMed);
        mol.setConcentration(mol.getConcentration() + delta);
      }    
  }
}
