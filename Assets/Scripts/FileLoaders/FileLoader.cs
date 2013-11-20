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
public class FileLoader
{
  private delegate void  StrSetter(string dst);
  private delegate void  FloatSetter(float dst);


  private PromoterLoader _promoterLoader;                       //!< The loader that will load everything about Promoter reactions
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
    \brief This fonction load a molecule
    \param node The xml node to parse
    \param type The molecule type
    \param molecules The array of molecules where to add new molecules
    \return Return always true
   */
  private bool storeMolecule(XmlNode node, Molecule.eType type, ArrayList molecules)
  {
    Molecule mol = new Molecule();

    mol.setType(type);
    foreach (XmlNode attr in node)
      {
        switch (attr.Name)
          {
          case "name":
            mol.setName(attr.InnerText);
            break;
          case "description":
            mol.setDescription(attr.InnerText);
            break;
          case "concentration":
            mol.setConcentration(float.Parse(attr.InnerText.Replace(",", ".")));
            break;
          case "degradationRate":
            mol.setDegradationRate(float.Parse(attr.InnerText.Replace(",", ".")));
            break;
          case "FickFactor":
            mol.setFickFactor(float.Parse(attr.InnerText.Replace(",", ".")));
            break;
          }
     }
    molecules.Add(mol);
    return true;
  }

  /*!
    \brief Load one molecule from an xml node
    \param node The xml node
    \param molecules The array where to add the new molecule
    \return always true
   */
  private bool loadMolecule(XmlNode node, ArrayList molecules)
  {
    if (node.Attributes["type"] == null)
      return false;
    switch (node.Attributes["type"].Value)
      {
      case "enzyme":
        return storeMolecule(node, Molecule.eType.ENZYME, molecules);
      case "transcription_factor":
        return storeMolecule(node, Molecule.eType.TRANSCRIPTION_FACTOR, molecules);
      case "other":
        return storeMolecule(node, Molecule.eType.OTHER, molecules);
      }
    return true;
  }

  /*!
    \brief Load molecules from files
    \param node The xml node
    \param molecules The array of molecule to fill
    \return Return always true
   */
  private bool loadMolecules(XmlNode node, ArrayList molecules)
  {
    foreach (XmlNode mol in node)
      {
        if (mol.Name == "molecule")
          loadMolecule(mol, molecules);
      }
    return true;
  }

  /*!
    \brief This function load reactions from xml node
    \param node The xml node
    \param reaction The list of reaction where to add new reactions
    \return Return Always true
   */
  private bool loadReactions(XmlNode node, LinkedList<IReaction> reactions)
  {
    _promoterLoader.loadPromoters(node, reactions);
    _enzymeReactionLoader.loadEnzymeReactions(node, reactions);
    _allosteryLoader.loadAllostericReactions(node, reactions);
    _instantReactionLoader.loadInstantReactions(node, reactions);
    return true;
  }

  /*!
    \brief Load reactions from a file
    \param filePath the path of the file that contains reactions
    \return Return a list of ReactionsSet
    \sa ReactionsSet
   */
  public LinkedList<ReactionsSet> loadReactionsFromFile(string filePath)
  {
    MemoryStream ms = Tools.getEncodedFileContent(filePath);
    bool b = true;
    ReactionsSet reactionSet;
    string setId;
    LinkedList<ReactionsSet> reactionSets = new LinkedList<ReactionsSet>();

    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.Load(ms);
    XmlNodeList reactionsLists = xmlDoc.GetElementsByTagName("reactions");
    foreach (XmlNode reactionsNode in reactionsLists)
      {
        setId = reactionsNode.Attributes["id"].Value;
        if (setId != "" && setId != null)
          {
            LinkedList<IReaction> reactions = new LinkedList<IReaction>();
            b &= loadReactions(reactionsNode, reactions);
            reactionSet = new ReactionsSet();
            reactionSet.id = setId;
            reactionSet.reactions = reactions;
            reactionSets.AddLast(reactionSet);
          }
        else
          {
            Debug.Log("Error : missing attribute id in reactions node");
            b = false;
          }
      }
    return reactionSets;
  }

  /*!
    \brief Load molecules from a file
    \param filePath the path of the file that contains molecules
    \return Return a list of MoleculesSet
    \sa MoleculesSet
   */
  public LinkedList<MoleculesSet> loadMoleculesFromFile(string filePath)
  {
    MemoryStream ms = Tools.getEncodedFileContent(filePath);
    bool b = true;
    MoleculesSet moleculeSet;
    string setId;
    LinkedList<MoleculesSet> moleculesSets = new LinkedList<MoleculesSet>();

    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.Load(ms);
    XmlNodeList moleculesLists = xmlDoc.GetElementsByTagName("molecules");
    foreach (XmlNode moleculesNode in moleculesLists)
      {
        setId = moleculesNode.Attributes["id"].Value;
        if (setId != "" && setId != null)
          {
            ArrayList molecules = new ArrayList();
            b &= loadMolecules(moleculesNode, molecules);
            moleculeSet = new MoleculesSet();
            moleculeSet.id = setId;
            moleculeSet.molecules = molecules;
            moleculesSets.AddLast(moleculeSet);
          }
        else
          {
            Debug.Log("Error : missing attribute id in reactions node");
            b = false;
          }
      }
    return moleculesSets;
  }

}