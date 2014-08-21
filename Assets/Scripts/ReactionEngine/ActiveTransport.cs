using UnityEngine;
using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/*!
 \brief This class manages all the Active Transport reaction
 
 
 */
public class ActiveTransport : XmlLoaderImpl
{    
    
  public override string xmlTag
  {
    get
    {
      return "ATProp";
    }
  }

  /*!
   \brief Load a list of properties in order to convert it into a ActiveTransportReaction
   \param props The list of ActiveTransportProperties
   \param mediums The list of mediums
   */
  public void loadActiveTransportReactionsFromProperties(LinkedList<ActiveTransportProperties> props, LinkedList<Medium> mediums)
  {
    ActiveTransportReaction reaction;
    Medium med;

    foreach (ActiveTransportProperties prop in props)
      {
        reaction = new ActiveTransportReaction();
        reaction.setName(prop.name);
        reaction.setSubstrate(prop.substrate);
        reaction.setEnzyme(prop.enzyme);
        reaction.setKcat(prop.Kcat);
        reaction.setEffector(prop.effector);
        reaction.setAlpha(prop.alpha);
        reaction.setBeta(prop.beta);
        reaction.setKm(prop.Km);
        reaction.setKi(prop.Ki);
        reaction.setEnergyCost(prop.energyCost);
        foreach (Product p in prop.products)
          reaction.addProduct(p);
        med = ReactionEngine.getMediumFromId(prop.srcMediumId, mediums);
        if (med == null)
        {
          Debug.Log("Cannot load Active Transport properties because the medium Id : " + prop.srcMediumId + " is unknown.");
          break;
        }
        reaction.setSrcMedium(med);
        med = ReactionEngine.getMediumFromId(prop.dstMediumId, mediums);
            if (med == null)
          {
            Debug.Log("Cannot load Active Transport properties because the medium Id : " + prop.dstMediumId + " is unknown.");
            break;
          }
        reaction.setDstMedium(med);
        med = ReactionEngine.getMediumFromId(prop.mediumId, mediums);
            if (med == null)
          {
            Debug.Log("Cannot load Active Transport properties because the medium Id : " + prop.mediumId + " is unknown.");
            break;
          }
        reaction.setMedium(med);
            med.addReaction(reaction);
        }
  }

  /*!
   \brief This function load properties from files and load it.
   \param filesPaths All the files to be loaded.
   \params mediums The list of all mediums.
   */
  public  void loadActiveTransportReactionsFromFiles(IEnumerable<string> filesPaths, LinkedList<Medium> mediums)
  {
        Logger.Log ("ActiveTransport::loadActiveTransportReactionsFromFiles starting", Logger.Level.DEBUG);

    LinkedList<ActiveTransportProperties> properties = getActiveTransportPropertiesFromFiles(filesPaths);
        loadActiveTransportReactionsFromProperties(properties, mediums);    
        
        Logger.Log ("ActiveTransport::loadActiveTransportReactionsFromFiles ends"
                    ,Logger.Level.DEBUG);
    }
    
    
    
    /*!
    \brief Load all the properties from multiple files
    \param files All the files
    \return A list of all the properties
   */
    public LinkedList<ActiveTransportProperties> getActiveTransportPropertiesFromFiles(IEnumerable<string> files)
    {
        
        Logger.Log ("ActiveTransport::getActiveTransportPropertiesFromFiles("
                    +Logger.EnumerableToString<string>(files)
                    +") starts"
                    ,Logger.Level.DEBUG);

        LinkedList<ActiveTransportProperties> propsList = new LinkedList<ActiveTransportProperties>();
        LinkedList<ActiveTransportProperties> newPropList;
        
        foreach (string file in files)
        {
          newPropList = loadObjectsFromFile<ActiveTransportProperties>(file,"activeTransports");
          if(null != newPropList)
          {
            LinkedListExtensions.AppendRange<ActiveTransportProperties>(propsList, newPropList);
          }
        }
        
        Logger.Log ("ActiveTransport::getActiveTransportPropertiesFromFiles("
                    +Logger.EnumerableToString<string>(files)
                    +") returns "+Logger.ToString<ActiveTransportProperties>(propsList)
                    ,Logger.Level.DEBUG);

        return propsList;
    }
}
