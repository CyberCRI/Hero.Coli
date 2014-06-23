using UnityEngine;
using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief This class load properties from XML files

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
            <Kcat>1</Kcat>                     -> Reaction constant of enzymatic reaction
            <effector>False</effector>         -> The Molecule that will inhibit the reaction or activate the reaction (False value is accepted to said that there is no effector)
            <alpha>1</alpha>                   -> Alpha parameter. Describe the competitivity of the effector (see execEnzymeReaction in EnzymeReaction)
            <beta>0</beta>                     -> Beta parameter. Describe the extend of inhibition or the extend of activation (see execEnzymeReaction in EnzymeReaction)
            <Km>1.3</Km>                       -> Km parameter. Affinity between substrate and enzyme
            <Ki>0.0000001</Ki>                 -> Ki parameter. Affinity between effector and enzyme
            <Products>                         -> List of the products
              <name>O</name>                   -> Name of the product molecule
            </Products>
          </ATProp>
        </activeTransports>

  \sa EnzymeReaction
  \author Pierre COLLET
 */


public class ActiveTransportLoader : XmlLoaderImpl
{
    
  public override string xmlTag
  {
    get
    {
        return "ATProp";
    }
  }

  /*!
    \brief Load all the properties from multiple files
    \param files All the files
    \return A list of all the propieties
   */
  public LinkedList<ActiveTransportProperties> getActiveTransportPropertiesFromFiles(IEnumerable files)
  {
    LinkedList<ActiveTransportProperties> propsList = new LinkedList<ActiveTransportProperties>();
    LinkedList<ActiveTransportProperties> newPropList;

    foreach (string file in files)
      {
			newPropList = loadObjectsFromFile<ActiveTransportProperties>(file,"activeTransports");
       // newPropList = loadActiveTransportPropertiesFromFile(file);
        if (newPropList != null)
          LinkedListExtensions.AppendRange<ActiveTransportProperties>(propsList, newPropList);
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
    \brief This function load all the products wich are in <Products> fields and store them in an ActiveTransportProperties.
    \param node The XmlNode corresponding to <Products> </Products>.
    \param AT The ActiveTransportProperties where to store the products
    \return Return true always (not really usefull)
   */
  private bool loadActiveTransportReactionProducts(XmlNode node, ActiveTransportProperties AT)
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
    \brief Load all the attributes of an ActiveTransportProperties.
    \return The ActiveTransportProperties object corresponding to the reaction describe in the node.
    \param node The XmlNode corresponding to the <ATProp> </ATProp> node.
   */
    public ActiveTransportProperties loadActiveTransportProperties(XmlNode node)
    {
        ActiveTransportProperties prop = new ActiveTransportProperties();

        loadActiveTransportProperties(node, prop);

        return prop;
    }

  /*!
    \brief Load all the attributes of an ActiveTransportProperties.
    \param node The XmlNode corresponding to the <ATProp> </ATProp> node.
    \param properties The properties to be initialized by this loading.
   */
  public void loadActiveTransportProperties(XmlNode node, ActiveTransportProperties properties)
  {   
        Logger.Log("ActiveTransportLoader.loadActiveTransportProperties("+node+", "+properties+") will load", Logger.Level.ERROR);

    foreach (XmlNode attr in node)
    {
      switch (attr.Name)
      {
        case "name":
          properties.name = checkActiveTransportString(attr.InnerText);
          break;
        case "substrate":
          properties.substrate = checkActiveTransportString(attr.InnerText);
          break;
        case "enzyme":
          properties.enzyme = checkActiveTransportString(attr.InnerText);
          break;
        case "Kcat":
          properties.Kcat = checkActiveTransportFloat(attr.InnerText);
          break;
        case "effector":
          properties.effector = checkActiveTransportString(attr.InnerText);
          break;
        case "alpha":
          properties.alpha = checkActiveTransportFloat(attr.InnerText);
          break;
        case "beta":
          properties.beta = checkActiveTransportFloat(attr.InnerText);
          break;
        case "Km":
          properties.Km = checkActiveTransportFloat(attr.InnerText);
          break;
        case "Ki":
          properties.Ki = checkActiveTransportFloat(attr.InnerText);
          break;
        case "Medium":
          properties.mediumId = checkActiveTransportInt(attr.InnerText);
          break;
        case "EnergyCost":
          properties.energyCost = checkActiveTransportFloat(attr.InnerText);
          break;
        case "MediumSrc":
          properties.srcMediumId = checkActiveTransportInt(attr.InnerText);
          break;
        case "MediumDst":
          properties.dstMediumId = checkActiveTransportInt(attr.InnerText);
          break;
        case "Products":
          loadActiveTransportReactionProducts(attr, properties);
          break;
      }
    }

        Logger.Log("ActiveTransportLoader.loadActiveTransportProperties(node, properties) loaded this="+this, Logger.Level.ERROR);

  }

}