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
public class ActiveTransportProprieties
{
  public string name;
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
}

/*!
 \brief This class manages all the Active Transport reaction
 \author Pierre COLLET
 \mail pierre.collet91@gmail.com
 */
public class ActiveTransport {

  private ActiveTransportLoader                 _loader;        //!< The file loader that reads proprieties from files
  private LinkedList<ActiveTransportReaction>   _reactions;     //!< The reaction list.

  //! Default Constructor
  public ActiveTransport()
  {
    _loader = new ActiveTransportLoader();
    _reactions = new LinkedList<ActiveTransportReaction>();
  }

  /*!
   \brief Load a list of propieties in order to convert it into a ActiveTransportReaction
   \param props The list of ActiveTransportProprieties
   \param mediums The list of mediums
   */
  public void loadActiveTransportReactionsFromProprieties(LinkedList<ActiveTransportProprieties> props, LinkedList<Medium> mediums)
  {
    ActiveTransportReaction reaction;
    Medium med;

    foreach (ActiveTransportProprieties prop in props)
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
        foreach (Product p in prop.products)
          reaction.addProduct(p);
        med = ReactionEngine.getMediumFromId(prop.srcMediumId, mediums);
        if (med == null)
          {
            Debug.Log("Cannot load Active Transport proprieties because the medium Id : " + prop.srcMediumId + " is unknown.");
            break;
          }
        reaction.setSrcMedium(med);
        med = ReactionEngine.getMediumFromId(prop.dstMediumId, mediums);
        if (med == null)
          {
            Debug.Log("Cannot load Active Transport proprieties because the medium Id : " + prop.dstMediumId + " is unknown.");
            break;
          }
        reaction.setDstMedium(med);
        _reactions.AddLast(reaction);
     }
  }

  /*!
   \brief This function load proprieties from files and load it.
   \param filesPaths All the files to be loaded.
   \params mediums The list of all mediums.
   */
  public  void loadActiveTransportReactionsFromFiles(IEnumerable filesPaths, LinkedList<Medium> mediums)
  {
    LinkedList<ActiveTransportProprieties> proprieties = _loader.getActiveTransportProprietiesFromFiles(filesPaths);
    loadActiveTransportReactionsFromProprieties(proprieties, mediums);
  }

  /*!
   \brief Do all the ActiveTransportReaction
   */
  public void react()
  {
    foreach (ActiveTransportReaction r in _reactions)
      r.react(null);
  }
}
