using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

/*!
  \brief This class parse and load instant reactions
  \details A Instant reaction should respect this syntax :

        <instantReaction>
          <name>Water</name>                        -> Name of the reaction
          <EnergyCost>0.1</EnergyCost>              -> Energy cost of the reaction
          <reactants>
            <reactant>
              <name>O</name>                        -> Reactant name
              <quantity>1</quantity>                -> Reactant coefficiant (how much of this to make products)
            </reactant>
            <reactant>
              <name>H</name>
              <quantity>2</quantity>
            </reactant>
          </reactants>
          <products>
            <product>
              <name>H2O</name>                      -> Product Name
              <quantity>1</quantity>                -> Product Quantity (how much is created)
            </product>
          </products>
        </instantReaction>

  \sa InstantReaction
  
 */
public class InstantReactionLoader
{
  /*!
    \brief Parse and load reactants of an InstantReaction
    \param node The xml node to parse
    \param ir the InstantReaction to initialize
    \return return always true
   */
  private bool loadInstantReactionReactants(XmlNode node, InstantReaction ir)
  {
    foreach (XmlNode attr in node)
      if (attr.Name == "reactant")
        loadInstantReactionReactant(attr, ir);
    return true;
  }

  /*!
    \brief Parse and load reactant of an InstantReaction
    \param node The xml node to parse
    \param ir the InstantReaction to initialize
    \return return always true
  */
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

  /*!
    \brief Parse and load reactant of an InstantReaction
    \param node The xml node to parse
    \param ir the InstantReaction to initialize
    \return return always true
  */
  private bool loadInstantReactionProducts(XmlNode node, InstantReaction ir)
  {
    foreach (XmlNode attr in node)
      if (attr.Name == "product")
        loadInstantReactionProduct(attr, ir);
    return true;
  }

  /*!
    \brief Parse and load products of an InstantReaction
    \param node The xml node to parse
    \param ir the InstantReaction to initialize
    \return return always true
  */
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

  /*!
    \brief Parse and load the energy cost of an InstantReaction
    \param value The value string
    \param ir the InstantReaction to initialize
    \return return always true
  */
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

  /*!
    \brief Parse and load the instants reactions from an xml node and push them in the given IReaction list
    \param node The XmlNode
    \param reactions The list of IReaction where to push the new InstantReaction
    \return return always true
  */
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
