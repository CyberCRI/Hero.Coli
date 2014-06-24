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
    \brief This function loads a molecule
    \param node The xml node to parse
    \param type The molecule type
    \param molecules The array of molecules where to add new molecules
    \return Return always true
   */
  public static bool storeMolecule(XmlNode node, ArrayList molecules)
  {

        Logger.Log ("FileLoader.storeMolecule("+Logger.ToString(node)
                    +", "+Logger.ToString<Molecule>("Molecule", molecules)
                    +")"
                    , Logger.Level.DEBUG);

    Molecule mol = new Molecule();

    mol.initFromLoad(node, null);

    molecules.Add(mol);
        
        Logger.Log ("FileLoader.storeMolecule(node"
                    +", "+Logger.ToString<Molecule>("Molecule", molecules)
                    +") loaded "+mol
                    , Logger.Level.TRACE);

    return true;
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
		string setId;
		LinkedList<T> objectList = new LinkedList<T>();
		foreach (XmlNode objectNode in objectNodeList)
		{
			setId = objectNode.Attributes["id"].Value;
			if(setId != "" && setId != null)
			{

				T t = new T();
				t.initializeFromXml(objectNode, setId);

				objectList.AddLast(t);

			}
			else
      {
				Debug.Log("Error : missing attribute id in reactions node");
			}


		}
		return objectList;
	}


}