using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

public class InstantReactionLoader
{
  private bool loadInstantReactionReactants(XmlNode node, InstantReaction ir)
  {
    foreach (XmlNode attr in node)
      if (attr.Name == "reactant")
        loadInstantReactionReactant(attr, ir);
    return true;
  }

  private bool loadInstantReactionReactant(XmlNode node, InstantReaction ir)
  {
    Product prod = new Product();
    foreach (XmlNode attr in node)
      {
        if (attr.Name == "name")
          {
            if (String.IsNullOrEmpty(attr.InnerText))
              Debug.Log("Warning : Empty name field in instant reaction reactant definition");
            prod.setName(attr.InnerText);
          }
        else if (attr.Name == "quantity")
          {
            if (String.IsNullOrEmpty(attr.InnerText))
              Debug.Log("Warning : Empty quantity field in instant reaction reactant definition");
            prod.setQuantityFactor(float.Parse(attr.InnerText.Replace(",", ".")));
          }
      }
    ir.addReactant(prod);
    return true;
  }

  private bool loadInstantReactionProducts(XmlNode node, InstantReaction ir)
  {
    foreach (XmlNode attr in node)
      if (attr.Name == "product")
        loadInstantReactionProduct(attr, ir);
    return true;
  }


  private bool loadInstantReactionProduct(XmlNode node, InstantReaction ir)
  {
    Product prod = new Product();
    foreach (XmlNode attr in node)
      {
        if (attr.Name == "name")
          {
            if (String.IsNullOrEmpty(attr.InnerText))
              Debug.Log("Warning : Empty name field in instant reaction product definition");
            prod.setName(attr.InnerText);
          }
        else if (attr.Name == "quantity")
          {
            if (String.IsNullOrEmpty(attr.InnerText))
              Debug.Log("Warning : Empty quantity field in instant reaction product definition");
            prod.setQuantityFactor(float.Parse(attr.InnerText.Replace(",", ".")));
          }
      }
    ir.addProduct(prod);
    return true;
  }

  private bool loadEnergyCost(string value, InstantReaction ir)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty EnergyCost field. default value = 0");
        ir.setEnergyCost(0f);
      }
    else
      ir.setEnergyCost(float.Parse(value.Replace(",", ".")));
    return true;
  }

  public bool loadInstantReactions(XmlNode node, LinkedList<IReaction> reactions)
  {
    XmlNodeList IReactionsList = node.SelectNodes("instantReaction");
    bool b = true;
    
    foreach (XmlNode IReaction in IReactionsList)
      {
        InstantReaction ir = new InstantReaction();
        foreach (XmlNode attr in IReaction)
          {
            switch (attr.Name)
              {
              case "name":
                ir.setName(attr.InnerText);
                break;
              case "reactants":
                loadInstantReactionReactants(attr, ir);
                break;
              case "products":
                loadInstantReactionProducts(attr, ir);
                break;
              case "EnergyCost":
                loadEnergyCost(attr.InnerText, ir);
                break;
              }
          }
        reactions.AddLast(ir);
      }
    return b;
  }
}
