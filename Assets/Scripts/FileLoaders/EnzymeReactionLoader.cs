using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

/*!
  \brief This class load all enzymatics reactions from a file
  \detail A enzymatic reaction should be declared by respecting this convention :

        <enzyme>
          <name>ER</name>                   -> Name of the reaction
          <EnergyCost>0</EnergyCost>        -> Energy cost of the reaction
          <substrate>X</substrate>          -> Substrate molecule's name
          <enzyme>E</enzyme>                -> Enzyme molecule's name
          <Kcat>10</Kcat>                   -> Reaction constant of enzymatic reaction
          <effector>False</effector>        -> The name of the effector (or false if there is no effector)
          <alpha>1000</alpha>               -> Competitive parameter (see EnzymeReaction class for more infos)
          <beta>0</beta>                    -> Activation or inhibition character of the effector (see EnzymeReaction class for more infos)
          <Km>0.5</Km>                      -> Affinity between substrate and enzyme
          <Ki>0.05</Ki>                     -> Affinity between effector and enzyme
          <Products>
            <name>X*</name>                 -> Molecule's name of the product
          </Products>
        </enzyme>

  \author Pierre COLLET
  \sa EnzymeReaction
 */
public class EnzymeReactionLoader
{
  private delegate void  StrSetter(string dst);
  private delegate void  FloatSetter(float dst);


  /*!
    \brief Load and parse a string and give it to the given setter
    \param value The string to parse and load
    \param setter The delegate setter
   */
  private bool loadEnzymeString(string value, StrSetter setter)
  {
    if (String.IsNullOrEmpty(value))
      return false;
    setter(value);
    return true;    
  }

  /*!
    \brief Load and parse a string and give it to the given setter
    \param value The string to parse and load
    \param setter The delegate setter
   */
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

  /*!
    \brief Load products of an enzymatic reaction
    \param node The xml node to load
    \param er The EnzymeReaction to initialize
    \return Return always true
   */
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

  /*!
    \brief Load all enzymatic reactions from an xml node
    \param node The xml node
    \param reactions The list of reactions
    \return Return true if succed, false otherwise
  */
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