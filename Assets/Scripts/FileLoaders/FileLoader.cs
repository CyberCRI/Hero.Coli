using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;

public class FileLoader
{
  private delegate void  StrSetter(string dst);
  private delegate void  FloatSetter(float dst);


  public TextAsset _reactionFile;
  private PromoterLoader _promoterLoader;
  private EnzymeReactionLoader _enzymeReactionLoader;
  private AllosteryLoader _allosteryLoader;
  private InstantReactionLoader _instantReactionLoader;

//   private PromoterParser _parser;

//   private LinkedList<IReaction> _reactions;
//   private ArrayList             _molecules;

  public FileLoader()
  {
    _promoterLoader = new PromoterLoader();
    _enzymeReactionLoader = new EnzymeReactionLoader();
    _allosteryLoader = new AllosteryLoader();
    _instantReactionLoader = new InstantReactionLoader();
//     _parser = new PromoterParser();
//     _reactions = new LinkedList<IReaction>();
//     _molecules = new ArrayList();
  }

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
          case "size":
            mol.setSize(float.Parse(attr.InnerText.Replace(",", ".")));
            break;
          }
     }
    molecules.Add(mol);
    //FIXME : create a real reaction for degradation with rate and name
//     _reactions.AddLast(new Degradation(mol.getDegradationRate(), mol.getName()));
    return true;
  }

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

  private bool loadMolecules(XmlNode node, ArrayList molecules)
  {
    foreach (XmlNode mol in node)
      {
        if (mol.Name == "molecule")
          loadMolecule(mol, molecules);
      }
    return true;
  }

//FIXME : patch parser to return correct boolean by checking if last node is a ending Token
  private bool loadReactions(XmlNode node, LinkedList<IReaction> reactions)
  {
    _promoterLoader.loadPromoters(node, reactions);
    _enzymeReactionLoader.loadEnzymeReactions(node, reactions);
    _allosteryLoader.loadAllostericReactions(node, reactions);
    _instantReactionLoader.loadInstantReactions(node, reactions);
    return true;
  }
  
  public LinkedList<ReactionsSet> loadReactionsFromFile(TextAsset filePath)
  {
    bool b = true;
    ReactionsSet reactionSet;
    string setId;
    LinkedList<ReactionsSet> reactionSets = new LinkedList<ReactionsSet>();

    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.LoadXml(filePath.text);
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

  public LinkedList<MoleculesSet> loadMoleculesFromFile(TextAsset filePath)
  {
    bool b = true;
    MoleculesSet moleculeSet;
    string setId;
    LinkedList<MoleculesSet> moleculesSets = new LinkedList<MoleculesSet>();

    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.LoadXml(filePath.text);
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