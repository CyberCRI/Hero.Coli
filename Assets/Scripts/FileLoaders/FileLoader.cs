using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;

/*!
  \brief This class load all the files needed by the reaction Engine
  \sa PromoterLoader
  \sa EnzymeReactionLoader
  \sa AllosteryLoader
  \sa InstantReactionLoader
  \author Pierre COLLET
 */


public class FileLoader : XmlLoader
{
  private delegate void  StrSetter(string dst);
  private delegate void  FloatSetter(float dst);


  private PromoterLoader _promoterLoader;                       //!< The loader that will load everything about PromoterReactions
  private EnzymeReactionLoader _enzymeReactionLoader;           //!< The loader that will load everything about Enzymes reactions
  private AllosteryLoader _allosteryLoader;                     //!< The loader that will load everything about allostery reactions
  private InstantReactionLoader _instantReactionLoader;         //!< The loader that will load everything about instant reactions

  //! Default constructor
  public FileLoader()
  {
    _promoterLoader = new PromoterLoader();
    _enzymeReactionLoader = new EnzymeReactionLoader();
    _allosteryLoader = new AllosteryLoader();
    _instantReactionLoader = new InstantReactionLoader();
  }

  /*!
    \brief This function load reactions from xml node
    \param node The xml node
    \param reaction The list of reaction where to add new reactions
    \return Return Always true
   */
  public bool loadReactions(XmlNode node, LinkedList<IReaction> reactions)
  {
    _promoterLoader.loadPromoters(node, reactions);
    _enzymeReactionLoader.loadEnzymeReactions(node, reactions);
    _allosteryLoader.loadAllostericReactions(node, reactions);
    _instantReactionLoader.loadInstantReactions(node, reactions);
    return true;
  }

  public override LinkedList<T> loadObjects<T> (XmlNodeList objectNodeList)
	{
		LinkedList<T> objectList = new LinkedList<T>();
		foreach (XmlNode objectNode in objectNodeList)
		{
			T t = new T();
      if(t.tryInstantiateFromXml(objectNode, null))
      {
        objectList.AddLast(t);
      }
      else
      {
        Logger.Log("FileLoader.loadObjects incorrect data in "+Logger.ToString (objectNode), Logger.Level.WARN);
      }  	
		}
		return objectList;
	}


}