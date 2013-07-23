using System;
using System.Collections;
using UnityEngine;

/*!
  \brief This class is a chemical reaction that simulate the production of ATP (energy)
  \details In order to consider energy in the reaction without implementing the reals
  really complex reactions that create ATP, this class is a really simple implementation
  of production of ATP.
  This class only create ATP without consuming anything else. If you want to implement the
  real reactions you can create a new reaction that inherit from IReaction class.
  \author Pierre COLLET
 */
public class ATPProducer : IReaction
{
  private float _production;            //!< Production speed

  public void setProduction(float v) { _production = v; }
  public float getProduction() { return _production; }

  //! Default Constructor
  public ATPProducer()
  {
    _production = 0f;
  }

  //! Copy Constructor
  public ATPProducer(ATPProducer r) : base(r)
  {
    _production = r._production;
  }

  /*!
    \brief This function is called at each step of time.
    \param molecules The list of molecules in the medium (useless here)
   */
  public override void react(ArrayList molecules)
  {
    _medium.addEnergy(_production * _reactionSpeed * ReactionEngine.reactionsSpeed);
  }
}