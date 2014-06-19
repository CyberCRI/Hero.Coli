using UnityEngine;
using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/*!
 \brief This class is a king of descriptor of ActiveTransportReaction
 \author Pierre COLLET
 \mail pierre.collet91@gmail.com
 \sa ActiveTransport
 \sa ActiveTransportReaction
 */
public class ActiveTransportProperties : XmlLoadableImpl
{
  public string name;
  public int mediumId;                    //!< The Medium where the reaction will be executed
  public int srcMediumId;            //!< The source medium for the transport
  public int dstMediumId;            //!< The destination medium for the transport
  public string substrate;            //!< The substrate of the reaction
  public string enzyme;               //!< The enzyme of the reaction
  public float Kcat;                  //!< The affinity between the substrate and the enzyme coefficient
  public string  effector;            //!< The effector of the reaction
  public float alpha;                 //!< Alpha descriptor of the effector
  public float beta;                  //!< Beta descriptor of the effector
  public float Km;                    //!< Affinity coefficient between substrate and enzyme
  public float Ki;                    //!< Affinity coefficient between effector and enzyme
  public LinkedList<Product> products;  //!< The list of the products
  public float energyCost;              //!< Cost in energy for one reaction
}

/*!
 \brief This class manages all the Active Transport reaction
 \author Pierre COLLET
 \mail pierre.collet91@gmail.com
 */
public class ActiveTransport {

  private ActiveTransportLoader                 _loader;        //!< The file loader that reads properties from files

  //! Default Constructor
  public ActiveTransport()
  {
    _loader = new ActiveTransportLoader();
  }

  /*!
   \brief Load a list of propieties in order to convert it into a ActiveTransportReaction
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
        //         _reactions.AddLast(reaction);
     }
  }

  /*!
   \brief This function load properties from files and load it.
   \param filesPaths All the files to be loaded.
   \params mediums The list of all mediums.
   */
  public  void loadActiveTransportReactionsFromFiles(IEnumerable filesPaths, LinkedList<Medium> mediums)
  {
    LinkedList<ActiveTransportProperties> properties = _loader.getActiveTransportPropertiesFromFiles(filesPaths);
    loadActiveTransportReactionsFromProperties(properties, mediums);
  }

	public ActiveTransportProperties initFromLoad(XmlNode node, ActiveTransportLoader loader)
	{
		return loader.loadActiveTransportProperties(node);
	}
}
