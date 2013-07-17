using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//!  
/*!
  \brief     Describe a FickReaction
  \details   This class is a descriptive class of a FickReaction
  \author    Pierre COLLET
  \mail      pierre.collet91@gmail.com
 */
public class FickProprieties
{
  public int MediumId1 {get; set;}
  public int MediumId2  {get; set;}
  public float P  {get; set;}
  public float surface  {get; set;}
  public float energyCost {get; set;}
}

/*!
 *  \brief     The class that manage all the diffusions reactions using Fick model.
 *  \details   This class initialize from files and execute all the FickReaction.
 *  \author    Pierre COLLET
 *  \mail      pierre.collet91@gmail.com
 */
public class Fick
{
  public const float MaximumMoleculeSize = 0.25f;       //!< Limit size of molecules that can cross the membrane of the Medium

  private LinkedList<FickReaction>      _reactions;     //!< The list of FickReaction
  FickLoader                            _loader;        //!< The class that load the FickReaction propieties

  //! Default constructor.
  public Fick()
  {
    _reactions = new LinkedList<FickReaction>();
    _loader = new FickLoader();
//     _mediums = mediums;
  }

  public LinkedList<FickReaction>       getFickReactions() { return _reactions; }

  //! return a FickReaction reference that correspondond to the two given ids
  /*!
      \param id1 The first id.
      \param id2 The second id.
      \param list The list of FickReaction where to search in.
    */
  public static FickReaction   getFickReactionFromIds(int id1, int id2, LinkedList<FickReaction> list)
  {
    foreach (FickReaction react in list)
      {
        Medium medium1 = react.getMedium1();
        Medium medium2 = react.getMedium2();
        if (medium1 != null && medium2 != null)
          {
          if ((medium1.getId() == id1 && medium2.getId() == id2) || (medium2.getId() == id1 && medium1.getId() == id2))
            return react;
          }
      }
    return null;
  }

  //! Set attributes of FickReactions by using a list of FickProprieties
  /*!
      \param propsList The list of Proprieties.
      \param FRList The list of FickReactions.
    */
  public static void           finalizeFickReactionFromProps(LinkedList<FickProprieties> propsList, LinkedList<FickReaction> FRList)
  {
    FickReaction react;

    foreach (FickProprieties prop in propsList)
      {
        react = getFickReactionFromIds(prop.MediumId1, prop.MediumId2, FRList);
        if (react != null)
          {
            react.setPermCoef(prop.P);
            react.setSurface(prop.surface);
          }
        else
          Debug.Log("da fuck dude!?");
      }
  }

  //! Load the diffusions reactions from a array of files and a Mediums list
  /*!
      \param files Array of files which contain information about diffusion reaction.
      \param mediums The list of all the medium.

This function load the diffusions reactions based on Fick model. It take a Array of file paths
and a list of Medium that should contain all the mediums of the simulation.
This function create the list of all the reactions between each Medium wich exist and initialize their parameters to 0.
Only the reactions explicitly defined in files are initialized to the values explicited in files.
If a parameter of a fick reaction is not specified in files then this parameter will be equal to 0.
    */
  public void           loadFicksReactionsFromFiles(string[] files, LinkedList<Medium> mediums)
  {
    LinkedList<FickProprieties> propsList = new LinkedList<FickProprieties>();
    LinkedList<FickProprieties> newPropList;

    foreach (string file in files)
      {
        newPropList = _loader.loadFickProprietiesFromFile(file);
        if (newPropList != null)
          LinkedListExtensions.AppendRange<FickProprieties>(propsList, newPropList);
      }
//     foreach (string file in files)
//       {
//         propsList = _loader.loadFickProprietiesFromFile(file);
        
//       }
    _reactions = FickReaction.getFickReactionsFromMediumList(mediums);
    finalizeFickReactionFromProps(propsList, _reactions);
  }

  //! This function is called at each frame and do all the reaction of type FickReactiono.
  public void react()
  {
    foreach (FickReaction fr in _reactions)
      fr.react(null);
  }
}

/*!
  \brief Represent a Fick's law 'reactions'.
  \details Manages Fick's law 'reactions'.
  This class manages a Fick's first law 'reaction', corresponding to the diffusion of
  small molecules across the cell membrane or between two different media.

  In this simulation, we model the rate of passive transport by diffusion across membranes.
  This rate depends on the concentration gradient between the two media and the (semi-)permeability
  of the membrane that separates them.  Diffusion moves materials from an area of higher concentration
  to the lower according to first Fick's law.
  A diffusion reaction, based on Fick's first law model, estimates the speed of diffusion of
  a molecule according to this formula : 

        dn/dt = (C1 - C2) * P * A

  - dn/dt: speed of diffusion
  - C1-C2: difference of concentration between both media
  - P: permeability coefficient
  - A exchange surface

  \reference http://books.google.fr/books?id=fXZW1REM0YEC&pg=PA636&lpg=PA636&dq=vitesse+de+diffusion+loi+de+fick&source=bl&ots=3eKv2NYYtx&sig=ciSW-RNAr0RTieE2oZfdBa73nT8&hl=en&sa=X&ei=bTufUcw4sI7sBqykgOAL&sqi=2&ved=0CD0Q6AEwAQ#v=onepage&q=vitesse%20de%20diffusion%20loi%20de%20fick&f=false
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
public class FickReaction : IReaction
{
  private float _surface;       //!< Contact surface size bwtween the two mediums
  private float _P;             //!< Permeability coefficient
  private Medium _medium1;      //!< The first Medium
  private Medium _medium2;      //!< The second Medium

//! Default constructor.
  public FickReaction()
  {
    _surface = 0;
    _P = 0;
    _medium1 = null;
    _medium2 = null;
  }

  public void setSurface(float surface) { _surface = surface;}
  public float getSurface() { return _surface;}
  public void setPermCoef(float P) { _P = P;}
  public float getPermCoef() { return _P;}
  public void setMedium1(Medium medium) { _medium1 = medium;}
  public Medium getMedium1() { return _medium1;}
  public void setMedium2(Medium medium) { _medium2 = medium;}
  public Medium getMedium2() { return _medium2;}

  public static IReaction        buildFickReactionFromProps(FickProprieties props, LinkedList<Medium> mediums)
  {
    FickReaction reaction = new FickReaction();
    Medium med1 = ReactionEngine.getMediumFromId(props.MediumId1, mediums);
    Medium med2 = ReactionEngine.getMediumFromId(props.MediumId2, mediums);

    if (med1 == null || med2 == null)
      {
        Debug.Log("failed to build FickReaction from FickProprieties beacause one or all the medium id don't exist");
        return null;
      }
    reaction.setSurface(props.surface);
    reaction.setPermCoef(props.P);
    reaction.setMedium1(med1);
    reaction.setMedium2(med2);
    reaction.setEnergyCost(props.energyCost);

    return reaction;
  }


  //! Return all the FickReactions possible from a Medium list.
  /*!
      \param mediums The list of mediums.

      \details
      This function return all the possible combinaisons of FickReaction in Medium list.

      Example :
        - Medium1 + Medium2 + Medium3 = FickReaction(1, 2) + FickReaction(1, 3) + FickReaction(2, 3)
  */
  public static LinkedList<FickReaction> getFickReactionsFromMediumList(LinkedList<Medium> mediums)
  {
    FickReaction                newReaction;
    LinkedListNode<Medium>      node;
    LinkedListNode<Medium>      start = mediums.First;
    LinkedList<FickReaction>    fickReactions = new LinkedList<FickReaction>();

    while (start != null)
      {
        node = start.Next;
        while (node != null)
          {
            newReaction = new FickReaction();
            newReaction.setMedium1(start.Value);
            newReaction.setMedium2(node.Value);
            fickReactions.AddLast(newReaction);
            node = node.Next;
          }
        start = start.Next;
      }
    return fickReactions;
  }

//! Processing a reaction.
  /*!
      \param molecules A list of molecules (not usefull here)

A diffusion reaction based on fick model is calculated by using this formula :
 dn/dt = c1 - c2 * P * A
Where:
        - dn is the difference of concentration that will be applied
        - c1 and c2 the concentration the molecules in the 2 Mediums
        - P is the permeability coefficient
        - A is the contact surface size between the two Mediums
  */
  public override void react(ArrayList molecules)
  {
    ArrayList molMed1 = _medium1.getMolecules();
    ArrayList molMed2 = _medium2.getMolecules();
    Molecule mol2;
    float c1;
    float c2;
    float result;

    if (_P == 0f || _surface == 0f)
      return;
    foreach (Molecule mol1 in molMed1)
      {
        c1 = mol1.getConcentration();
        mol2 = ReactionEngine.getMoleculeFromName(mol1.getName(), molMed2);
        if (mol2 != null && mol2.getSize() <= Fick.MaximumMoleculeSize)
          {
            c2 = mol2.getConcentration();
            result = (c2 - c1) * _P * _surface * _reactionSpeed * ReactionEngine.reactionsSpeed;
            if (enableNoise)
              {
                float noise = _numberGenerator.getNumber();
                result += noise;
              }
            if (enableSequential)
              {
                mol2.addConcentration(- result);
                mol1.addConcentration(result);
              }
            else
              {
                mol2.subNewConcentration(result);
                mol1.addNewConcentration(result);
              }
          }
      }
  }
}