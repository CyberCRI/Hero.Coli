using UnityEngine;
using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief This class load proprieties from XML files

  \important
  A file should respects this convention :

  <activeTransports>
   <ATProp>
     <name>name</name>                  -> The name
     <Medium>1<Medium>                  -> In wich medium the reaction is done.
     <MediumSrc>1</MediumSrc>           -> Medium where the reaction takes place
     <MediumDst>2</MediumDst>           -> Medium where the product will be released
     <EnergyCost>0.1</EnergyCost>       -> Energy that cost a reaction
     <substrate>S</substrate>           -> The Molecule that will be consumed in the SrcMedium
     <enzyme>E</enzyme>                 -> The Molecule wich will play the role of the tunnel
     <Kcat>1</Kcat>                     -> 
     <effector>False</effector>         -> The Molecule that will inhibit the reaction or activate the reaction (False value is accepted to said that there is no effector)
     <alpha>1</alpha>                   -> Alpha parameter. Describe the competitivity of the effector (see execEnzymeReaction in EnzymeReaction)
     <beta>0</beta>                     -> Beta parameter. Describe the extend of inhibition or the extend of activation (see execEnzymeReaction in EnzymeReaction)
     <Km>1.3</Km>                       -> Km parameter. Affinity between substrate and enzyme
     <Ki>0.0000001</Ki>                 -> Ki parameter. Affinity between effector and enzyme
     <Products>                         -> List of the products
       <name>O</name>
     </Products>
   </ATProp>
  </activeTransports>

  \sa EnzymeReaction
  \author Pierre COLLET
 */
public class ActiveTransportLoader
{
  /*!
    \brief Load all the proprieties from a file
    \param The path of the file
    \return The list of ActiveTranportProprieties. If the list is empty this function return null
   */
  public LinkedList<ActiveTransportProprieties> loadActiveTransportProprietiesFromFile(string filePath)
  {
    LinkedList<ActiveTransportProprieties>      propsList = new LinkedList<ActiveTransportProprieties>();
    ActiveTransportProprieties                  prop;

    MemoryStream ms = Tools.getEncodedFileContent(filePath);
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.Load(ms);

    XmlNodeList ATsLists = xmlDoc.GetElementsByTagName("activeTransports");
    foreach (XmlNode ATsNodes in ATsLists)
      {
        foreach (XmlNode ATNode in ATsNodes)
          {
            if (ATNode.Name == "ATProp")
              {
                prop = loadActiveTransportProprieties(ATNode);
                propsList.AddLast(prop);
              }
          }
      }

    if (propsList.Count == 0)
      return null;
    return propsList;
  }

  /*!
    \brief Load all the proprieties from multiple files
    \param files All the files
    \return A list of all the propieties
   */
  public LinkedList<ActiveTransportProprieties> getActiveTransportProprietiesFromFiles(IEnumerable files)
  {
    LinkedList<ActiveTransportProprieties> propsList = new LinkedList<ActiveTransportProprieties>();
    LinkedList<ActiveTransportProprieties> newPropList;

    foreach (string file in files)
      {
        newPropList = loadActiveTransportProprietiesFromFile(file);
        if (newPropList != null)
          LinkedListExtensions.AppendRange<ActiveTransportProprieties>(propsList, newPropList);
      }
    return propsList;
  }

  /*!
    \brief Check the validity of a string
    \param value the string to check
    \return Return the string if everything is ok or a empty string else
   */
  private string checkActiveTransportString(string value)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty field");
        return "";
      }
    return value;
  }

  /*!
    \brief Check the validity of a string to get parsed by float.Parse and return the float that corresponding
    \param value the string to check
    \return Return the corresponding float if everything is ok or 0 else
   */
  private float checkActiveTransportFloat(string value)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty field");
        return 0f;
      }
    return(float.Parse(value.Replace(",", ".")));
  }

  /*!
    \brief Check the validity of a string to get parsed by int.Parse and return the int that corresponding
    \param value the string to check
    \return Return the corresponding int if everything is ok or 0 else
   */
  private int checkActiveTransportInt(string value)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty field");
        return 0;
      }
    return (int.Parse(value));
  }

  /*!
    \brief This function load all the products wich are in <Products> fields and store them in an ActiveTransportProprieties.
    \param node The XmlNode corresponding to <Products> </Products>.
    \param AT The ActiveTransportProprieties where to store the products
    \return Return true always (not really usefull)
   */
  private bool loadActiveTransportReactionProducts(XmlNode node, ActiveTransportProprieties AT)
  {
    AT.products = new LinkedList<Product>();
    foreach (XmlNode attr in node)
      {
        if (attr.Name == "name")
          {
            if (String.IsNullOrEmpty(attr.InnerText))
              Debug.Log("Warning : Empty name field in ActiveTransport Reaction definition");
            Product prod = new Product();
            prod.setName(node.InnerText);
            AT.products.AddLast(prod);
          }
      }
    return true;
  }

  /*!
    \brief Load all the attributes of an ActiveTransportProprieties.
    \return The ActiveTransportProprieties object corresponding to the reaction describe in the node.
    \param node The XmlNode corresponding to the <ATProp> </ATProp> node.
   */
  private ActiveTransportProprieties loadActiveTransportProprieties(XmlNode node)
  {
    ActiveTransportProprieties prop = new ActiveTransportProprieties();

    foreach (XmlNode attr in node)
      {
        switch (attr.Name)
          {
          case "name":
            prop.name = checkActiveTransportString(attr.InnerText);
            break;
          case "substrate":
            prop.substrate = checkActiveTransportString(attr.InnerText);
            break;
          case "enzyme":
            prop.enzyme = checkActiveTransportString(attr.InnerText);
            break;
          case "Kcat":
            prop.Kcat = checkActiveTransportFloat(attr.InnerText);
            break;
          case "effector":
            prop.effector = checkActiveTransportString(attr.InnerText);
            break;
          case "alpha":
            prop.alpha = checkActiveTransportFloat(attr.InnerText);
            break;
          case "beta":
            prop.beta = checkActiveTransportFloat(attr.InnerText);
            break;
          case "Km":
            prop.Km = checkActiveTransportFloat(attr.InnerText);
            break;
          case "Ki":
            prop.Ki = checkActiveTransportFloat(attr.InnerText);
            break;
          case "Medium":
            prop.mediumId = checkActiveTransportInt(attr.InnerText);
            break;
          case "EnergyCost":
            prop.energyCost = checkActiveTransportFloat(attr.InnerText);
            break;
          case "MediumSrc":
            prop.srcMediumId = checkActiveTransportInt(attr.InnerText);
            break;
          case "MediumDst":
            prop.dstMediumId = checkActiveTransportInt(attr.InnerText);
            break;
          case "Products":
            loadActiveTransportReactionProducts(attr, prop);
            break;
          }
      }
    return prop;
  }

}