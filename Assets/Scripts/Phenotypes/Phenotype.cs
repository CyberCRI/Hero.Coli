using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief This class represents the interface between the Reaction Engine and the game.
  \details Each class that uses molecular concentrations to perform a graphical effect
  should inherit from this class. This abstract class implements the basic stuff that a phenotype needs.
  It asks the reaction engine the concentration of each protein of the medium it represents.

  Some attributes have to be defined before runing it :

                - A reference to the reaction engine
                - A mediumId

 This class also implements useful functions like a hill function.
 
 
 \sa ReactionEngine
 \sa Medium
 */
public abstract class Phenotype : MonoBehaviour
{
  protected ReactionEngine _reactionEngine;           //!< The ReactionEngine (ReactionEngine)
  private static int       _characterMediumID = 1;         //!< The medium id of the character
  private int              _mediumId = _characterMediumID; //!< The medium id

  protected static ArrayList _molecules;
  protected Medium           _mediumRef;


  public virtual void initialize() {
    _reactionEngine = null;
    _mediumRef = null;
    _molecules = null;
  }

  //! \brief Set the medium id.
  //! \param id The id.
  public void   setMediumId(int id)
  {
    _mediumId = id;
  }

  /*!
    \brief Execute a hill function.
    \param x The variable of hill function (commonly the concentration of the molecule)
    \param threshold The threshold of the function (K)
    \param steepness The steepness of the Hill function (n)
    \param min The minimum value to return
    \param max the maximum value to return
    \return Return the value of the Hill function between min and max value.
   */
  public static float hill(float x, float threshold, float steepness, float min, float max)
  {
    return (float)(Math.Pow(x, steepness) / (threshold + Math.Pow(x, steepness)) * (max - min) + min);
  }

  /*!
    \brief This method gets all molecular concentrations
    \sa Molecule
    \sa Molecule.getConcentration()
   */
  public void initMolecules()
  {
    LinkedList<Medium>    mediums = _reactionEngine.getMediumList();
    _mediumRef = ReactionEngine.getMediumFromId(_mediumId, mediums);

    if(_mediumRef != null && _molecules == null)
    {
      _molecules = _mediumRef.getMolecules();
    }
  }

  /*!
    \brief Called at the beginning.
   */
  public void Start () {
    initialize();
    _reactionEngine = ReactionEngine.get();
    StartPhenotype();
	  initMolecules();
  }
  
  /*!
    \brief Called at each tick of the game.
   */
  public virtual void Update () {
    if(!ReactionEngine.isPaused())
    {
      UpdatePhenotype();
    }
  }

  public abstract void StartPhenotype();
  public abstract void UpdatePhenotype();
}
