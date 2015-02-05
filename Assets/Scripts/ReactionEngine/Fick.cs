using UnityEngine;
using System.Collections.Generic;
using System.Collections;


/*!
 *  \brief     The class that manages all the diffusion reactions using Fick model.
 *  \details   This class initializes from files and executes all the FickReaction.
 *  
 *  
 */
public class Fick : XmlLoaderImpl
{
  public const float MaximumMoleculeSize = 0.25f;       //!< Limit size of molecules that can cross the membrane of the Medium

  private LinkedList<FickReaction>      _reactions;     //!< The list of FickReaction

  public override string xmlTag
  {
    get
    {
      return "fickProp";
    }
  }


  //! Default constructor.
  public Fick()
  {
    _reactions = new LinkedList<FickReaction>();
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

  //! Set attributes of FickReactions by using a list of FickProperties
  /*!
      \param propsList The list of Properties.
      \param FRList The list of FickReactions.
    */
  public static void           finalizeFickReactionFromProps(LinkedList<FickProperties> propsList, LinkedList<FickReaction> FRList)
  {
    FickReaction react;

    foreach (FickProperties prop in propsList)
      {
        react = getFickReactionFromIds(prop.MediumId1, prop.MediumId2, FRList);
        if (react != null)
          {
            react.setPermCoef(prop.P);
            react.setSurface(prop.surface);
          }
        else
          Debug.Log("Cannot initialize this fick reaction because there is some non-logical declaration of FickReaction. Please verify your files");
      }
  }

  //! Load the diffusion reactions from an array of files and a Medium list
  /*!
      \param files Array of files which contain information about diffusion reaction.
      \param mediums The list of all the mediums.

      This function loads the diffusion reactions based on Fick model. It takes an Array of file paths
      and a list of Medium that should contain all the mediums of the simulation.
      This function creates the list of all the reactions between all Medium which exist and initialize their parameters to 0.
      Only the reactions explicitly defined in files are initialized to the values explicited in files.
      If a parameter of a fick reaction is not specified in files then this parameter will be equal to 0.
    */
  public void loadFicksReactionsFromFiles (string[] files, LinkedList<Medium> mediums)
    {
        Logger.Log ("Fick::loadFicksReactionsFromFiles("
            + Logger.EnumerableToString<string> (files)
            + ") starts"
                    , Logger.Level.INFO);
        LinkedList<FickProperties> propsList = new LinkedList<FickProperties> ();
        LinkedList<FickProperties> newPropList;

        foreach (string file in files) {
            newPropList = loadObjectsFromFile<FickProperties> (file, "ficks");
            if (newPropList != null)
                LinkedListExtensions.AppendRange<FickProperties> (propsList, newPropList);
        }
        _reactions = FickReaction.getFickReactionsFromMediumList (mediums);
        finalizeFickReactionFromProps (propsList, _reactions);
        
        Logger.Log ("Fick::loadFicksReactionsFromFiles("
            + Logger.EnumerableToString<string> (files)
            + ") starts"
                    , Logger.Level.INFO);
    }

    //! This function is called at each frame and does all the reactions of type FickReaction.
    public void react ()
    {
        foreach (FickReaction fr in _reactions)
            fr.react (null);
    }
}