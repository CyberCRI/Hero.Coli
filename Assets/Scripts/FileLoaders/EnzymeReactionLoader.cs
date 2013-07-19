using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

public class EnzymeReactionLoader
{
  private delegate void  StrSetter(string dst);
  private delegate void  FloatSetter(float dst);


  private bool loadEnzymeString(string value, StrSetter setter)
  {
    if (String.IsNullOrEmpty(value))
      return false;
    setter(value);
    return true;    
  }

  private bool loadEnzymeFloat(string value, FloatSetter setter)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty productionMax field");
        return false;
      }
    setter(float.Parse(value.Replace(",", ".")));
    return true;    
  }

  private bool loadEnzymeReactionProducts(XmlNode node, EnzymeReaction er)
  {
    foreach (XmlNode attr in node)
      {
        if (attr.Name == "name")
          {
            if (String.IsNullOrEmpty(attr.InnerText))
              Debug.Log("Warning : Empty name field in Enzyme Reaction definition");
            Product prod = new Product();
            prod.setName(node.InnerText);
            er.addProduct(prod);
          }
      }
    return true;
  }

  public bool loadEnzymeReactions(XmlNode node, LinkedList<IReaction> reactions)
  {
    XmlNodeList EReactionsList = node.SelectNodes("enzyme");
    bool b = true;
    
    foreach (XmlNode EReaction in EReactionsList)
      {
        EnzymeReaction er = new EnzymeReaction();
        foreach (XmlNode attr in EReaction)
          {
            switch (attr.Name)
              {
              case "name":
                b = b && loadEnzymeString(attr.InnerText, er.setName);
                break;
              case "substrate":
                b = b && loadEnzymeString(attr.InnerText, er.setSubstrate);
                break;
              case "enzyme":
                b = b && loadEnzymeString(attr.InnerText, er.setEnzyme);
                break;
              case "Kcat":
                b = b && loadEnzymeFloat(attr.InnerText, er.setKcat);
                break;
              case "effector":
                b = b && loadEnzymeString(attr.InnerText, er.setEffector);
                break;
              case "alpha":
                b = b && loadEnzymeFloat(attr.InnerText, er.setAlpha);
                break;
              case "EnergyCost":
                b = b && loadEnzymeFloat(attr.InnerText, er.setEnergyCost);
                break;
              case "beta":
                b = b && loadEnzymeFloat(attr.InnerText, er.setBeta);
                break;
              case "Km":
                b = b && loadEnzymeFloat(attr.InnerText, er.setKm);
                break;
              case "Ki":
                b = b && loadEnzymeFloat(attr.InnerText, er.setKi);
                break;
              case "Products":
                b = b && loadEnzymeReactionProducts(attr, er);
                break;
              }
          }
        reactions.AddLast(er);
      }
    return b;
  }
}