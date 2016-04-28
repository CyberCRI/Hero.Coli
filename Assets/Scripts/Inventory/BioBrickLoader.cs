using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;

//TODO refactor with FileLoader
public class BioBrickLoader {

  string bioBrickName;
  string bioBrickSize;
  string bioBrickType;

  string beta;
  string formula;
  string rbsfactor;
  string protein;
  string terminatorfactor;

  BioBrick bioBrick;

  private void reinitVars() {
    bioBrickName = null;
    bioBrickSize = null;
    bioBrickType = null;

    beta = null;
    formula = null;
    rbsfactor = null;
    protein = null;
    terminatorfactor = null;

    bioBrick = null;
  }

  public BioBrickLoader() {
    Logger.Log("BioBrickLoader::BioBrickLoader()");
  }

  public BioBrick loadBioBrick(XmlNode bioBrickNode)
  {
      
      reinitVars();
      //common biobrick attributes
      try {
        bioBrickName = bioBrickNode.Attributes[BioBricksXMLTags.ID].Value;
        bioBrickSize = bioBrickNode.Attributes[BioBricksXMLTags.SIZE].Value;
        bioBrickType = bioBrickNode.Attributes[BioBricksXMLTags.TYPE].Value;
      }
      catch (NullReferenceException exc) {
        Logger.Log("BioBrickLoader::loadBioBricksFromFile bad xml, missing field\n"+exc, Logger.Level.WARN);
        return null;
      }
      catch (Exception exc) {
        Logger.Log("BioBrickLoader::loadBioBricksFromFile failed, got exc="+exc, Logger.Level.WARN);
        return null;
      }
      Logger.Log ("BioBrickLoader::loadBioBricksFromFile got id="+bioBrickName
        +", size="+bioBrickSize
        +", type="+bioBrickType
        , Logger.Level.TRACE);

      if (checkString(bioBrickName)) {
        switch(bioBrickType) {
          case BioBricksXMLTags.PROMOTER:
            logCurrentBioBrick(BioBricksXMLTags.PROMOTER);
            foreach (XmlNode attr in bioBrickNode){
              switch (attr.Name){
                case BioBricksXMLTags.BETA:
                    beta = attr.InnerText;
                  break;
                case BioBricksXMLTags.FORMULA:
                  formula = attr.InnerText;
                  break;
                case XMLTags.COMMENT:
                  break;
                default:
                    logUnknownAttr(attr, BioBricksXMLTags.PROMOTER);
                  break;
              }
            }
            if(checkString(beta) && checkString(formula)){
              bioBrick = new PromoterBrick(bioBrickName, parseFloat(beta), formula);
            }
            break;
          case BioBricksXMLTags.RBS:
            logCurrentBioBrick(BioBricksXMLTags.RBS);
              foreach (XmlNode attr in bioBrickNode){
              switch (attr.Name){
                case BioBricksXMLTags.RBSFACTOR:
                  rbsfactor = attr.InnerText;
                    break;
                case XMLTags.COMMENT:
                    break;
                default:
                  logUnknownAttr(attr, BioBricksXMLTags.RBS);
                  break;
              }
              }
            if(checkString(rbsfactor)) {
              bioBrick = new RBSBrick(bioBrickName, parseFloat(rbsfactor));
            }
            break;
          case BioBricksXMLTags.GENE:
            logCurrentBioBrick(BioBricksXMLTags.GENE);
            foreach (XmlNode attr in bioBrickNode){
              switch (attr.Name){
                case BioBricksXMLTags.PROTEIN:
                  protein = attr.InnerText;
                    break;
                case XMLTags.COMMENT:
                    break;
                default:
                  logUnknownAttr(attr, BioBricksXMLTags.GENE);
                  break;
              }
              }
            if(checkString(protein)) {
              bioBrick = new GeneBrick(bioBrickName, protein);
            }
            break;
          case BioBricksXMLTags.TERMINATOR:
              logCurrentBioBrick(BioBricksXMLTags.TERMINATOR);
            foreach (XmlNode attr in bioBrickNode){
              switch (attr.Name){
                case BioBricksXMLTags.TERMINATORFACTOR:
                  terminatorfactor = attr.InnerText;
                    break;
                case XMLTags.COMMENT:
                    break;
                default:
                    logUnknownAttr(attr, BioBricksXMLTags.TERMINATOR);
                  break;
              }
            }
            if(checkString(terminatorfactor)) {
              bioBrick = new TerminatorBrick(bioBrickName, parseFloat(terminatorfactor));
            }
              break;
          default:
              Logger.Log ("BioBrickLoader::loadBioBricksFromFile wrong type "+bioBrickType, Logger.Level.WARN);
            break;
        }
        if(null != bioBrick && checkString(bioBrickSize)){
          bioBrick.setSize(parseInt(bioBrickSize));
        }
        return bioBrick;
        } else {
          Logger.Log("BioBrickLoader::loadBioBricksFromFile Error : missing attribute id in BioBrick node", Logger.Level.WARN);
          return null;
        }
  }

  public LinkedList<BioBrick> loadBioBricksFromFile(string filePath)
  {
    Logger.Log("BioBrickLoader::loadBioBricksFromFile("+filePath+")", Logger.Level.INFO);

    LinkedList<BioBrick> resultBioBricks = new LinkedList<BioBrick>();

    XmlDocument xmlDoc = Tools.getXmlDocument(filePath);
    
    XmlNodeList bioBrickList = xmlDoc.GetElementsByTagName(BioBricksXMLTags.BIOBRICK);

    foreach (XmlNode bioBrickNode in bioBrickList)
    {
        loadBioBrick(bioBrickNode);
        if(bioBrick != null){
          resultBioBricks.AddLast(bioBrick);
        }
    }
    return resultBioBricks;
  }

  private static float parseFloat(string real) {
    return float.Parse(real.Replace (",", "."));
  }

  private static int parseInt(string integer) {
    return int.Parse(integer);
  }

  private static void logUnknownAttr(XmlNode attr, string type) {
    Logger.Log("BioBrickLoader::loadBioBricksFromFile unknown attr "+attr.Name+" for "+type+" node", Logger.Level.WARN);
  }

  private static void logCurrentBioBrick(string type){
    Logger.Log("BioBrickLoader::loadBioBricksFromFile type="+type, Logger.Level.TRACE);
  }

  private bool checkString(string toCheck) {
    return !String.IsNullOrEmpty(toCheck);
  }

}
