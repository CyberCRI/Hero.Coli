using System.IO;
using System.Collections.Generic;
using System.Xml;
using System;
using UnityEngine;

/*! 
 *  \brief     Load medium files
 *  \details   This class load everything about mediums from medium files
 A medium file should respect this synthax :

<Mediums>
  <Medium type="Cellia">
    <Id>01</Id>                                         -> Unique ID of the medium
    <Name>Cellia</Name>                                 -> Name of the medium
    <ReactionsSet>CelliaReactions</ReactionsSet>        -> ReactionsSet to load in the medium
    <MoleculesSet>CelliaMolecules</MoleculesSet>        -> MoleculesSet to load in the medium
    <Energy>1000</Energy>                               -> Initial Energy
    <MaxEnergy>2000</MaxEnergy>                         -> Maximal energy
    <EnergyProductionRate>10</EnergyProductionRate>     -> The energy production speed
  </Medium>
</Mediums>

 *  \author    Pierre COLLET
 *  \sa ReactionsSet
 *  \sa MoleculesSet
 *  \sa Medium
 */
public class MediumLoader
{

  /*!
    \brief This function load the initial energy of the medium and parse the validity of the given string
    \param value The value to parse and load
    \param med The medium to initialize
    \return Return true if the function succed to parse the string or false else
   */
  private bool loadEnergy(string value, Medium med)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty Energy field. default value = 0");
        med.setEnergy(0f);
      }
    else
      med.setEnergy(float.Parse(value.Replace(",", ".")));
    return true;
  }

  /*!
    \brief This function load the energy production rate of the medium and parse the validity of the given string
    \param value The value to parse and load
    \param med The medium to initialize
    \return Return true if the function succed to parse the string or false else
   */
  private bool loadEnergyProductionRate(string value, Medium med)
  {
    float productionRate;

    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty EnergyProductionRate field. default value = 0");
        productionRate = 0f;
      }
    else
      productionRate = float.Parse(value.Replace(",", ".")); 
    med.setEnergyProductionRate(productionRate);
    return true;
  }

  /*!
    \brief This function load the maximum energy in the medium and parse the validity of the given string
    \param value The value to parse and load
    \param med The medium to initialize
    \return Return true if the function succed to parse the string or false else
   */
  private bool loadMaxEnergy(string value, Medium med)
  {
    float prodMax;

    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty EnergyProductionRate field. default value = 0");
        prodMax = 0f;
      }
    else
      prodMax = float.Parse(value.Replace(",", ".")); 
    med.setMaxEnergy(prodMax);
    return true;
  }

  /*!
    \brief This function create a new Medium based on the information in the given XML Node
    \param node The XmlNode to load.
    \return Return the new Medium
  */
  public Medium   loadMedium(XmlNode node)
  {
    Medium medium = new Medium();

    foreach (XmlNode attr in node)
      {
        switch (attr.Name)
          {
          case "Id":
            medium.setId(Convert.ToInt32(attr.InnerText));
            break;
          case "Name":
            medium.setName(attr.InnerText);
            break;
          case "Energy":
            loadEnergy(attr.InnerText, medium);
            break;
          case "EnergyProductionRate":
            loadEnergyProductionRate(attr.InnerText, medium);
            break;
          case "MaxEnergy":
            loadMaxEnergy(attr.InnerText, medium);
            break;
          case "ReactionsSet":
            medium.setReactionsSet(attr.InnerText);
            break;
          case "MoleculesSet":
            medium.setMoleculesSet(attr.InnerText);
            break;
          }
      }
    return medium;
  }

  /*!
    \brief Create a list of Medium declared in the given file
    \param filePath The path of the file to load
    \return Return a LinkedList of Medium or null if no medium are declared in the given file
   */
  public LinkedList<Medium>     loadMediumsFromFile(string filePath)
  {

    LinkedList<Medium> mediums = new LinkedList<Medium>();
    Medium medium;


    MemoryStream ms = Tools.getEncodedFileContent(filePath);
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.Load(ms);

    XmlNodeList mediumsLists = xmlDoc.GetElementsByTagName("Mediums");
    foreach (XmlNode mediumNodes in mediumsLists)
      {
        foreach (XmlNode mediumNode in mediumNodes)
          {
            medium = loadMedium(mediumNode);
            mediums.AddLast(medium);
          }
      }

    if (mediums.Count == 0)
      return null;
    return mediums;
  }
}