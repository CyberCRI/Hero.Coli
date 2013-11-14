using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief This class represents the interface between the Reaction Engine and the game.
  \details Each class that uses molecular concentration to perform a graphical effect
  should inherit from this class. This abstract class implements the basic stuff that a phenotype needs.
  It asks the reaction engine the concentration of each protein of the medium it represents.

  Some attributes have to be define before runing it :

                - A reference to the reaction engine
                - A mediumId

 This class also implements useful functions like a hill function.
 \author Pierre COLLET
 \mail pierre.collet91@gmail.com
 \sa ReactionEngine
 \sa Medium
 */
public abstract class Phenotype : MonoBehaviour
{
  public ReactionEngine _RE;                    //!< The ReactionEngine (ReactionEngine)
  public int            _mediumId;              //!< The medium id

  protected ArrayList             _molecules;
  protected Medium                _mediumRef;
  protected bool                  _isIdChanged;

  //! \brief Set the medium id.
  //! \param id The id.
  public void   setMediumId(int id)
  {
    _mediumId = id;
    _isIdChanged = true;
  }

  /*!
    \brief Execute a hill function.
    \param x The variable of hill function (commonly the concentration of the molecule)
    \param threshold The threshold of the function (K)
    \param stepness The stepness of the Hill function (n)
    \param min The minimum value to return
    \param max the maximum value to return
    \return Return the value of the Hill function between min and max value.
   */
  public static float hill(float x, float threshold, float stepness, float min, float max)
  {
    return (float)(Math.Pow(x, stepness) / (threshold + Math.Pow(x, stepness)) * (max - min) + min);
  }

  /*!
    \brief This class updates all molecular concentrations
    \sa Molecule
    \sa Molecule.getConcentration()
   */
  public void updateMolecules()
  {
    if (_isIdChanged)
      {
        LinkedList<Medium>    mediums = _RE.getMediumList();
        _mediumRef = ReactionEngine.getMediumFromId(_mediumId, mediums);
        _isIdChanged = false;
      }
    _molecules = _mediumRef.getMolecules();    
  }

  /*!
    \brief Called at the beginning.
   */
  public void Start () {
    _isIdChanged = true;
    StartPhenotype();
  }
  
  /*!
    \brief Called at each tick of the game.
   */
  public void Update () {
    updateMolecules();
    UpdatePhenotype();
  }

  public abstract void StartPhenotype();
  public abstract void UpdatePhenotype();
}
