using System.IO;
using System.Collections.Generic;
using System.Xml;
using System;
using UnityEngine;

/*! 
 *  \brief     Load medium files
 *  \details   This class load everything about mediums from medium files
 *  \author    Pierre COLLET
 */
public class MediumLoader
{

  
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

  public LinkedList<Medium>     loadMediumsFromFile(string filePath)
  {

    LinkedList<Medium> mediums = new LinkedList<Medium>();
    Medium medium;


    MemoryStream ms = Tools.getEncodedFileContent(filePath);
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.Load(ms);

//     XmlDocument xmlDoc = new XmlDocument();
//     Debug.Log(filePath);
//     xmlDoc.LoadXml(filePath);
    XmlNodeList mediumsLists = xmlDoc.GetElementsByTagName("Mediums");
    foreach (XmlNode mediumNodes in mediumsLists)
      {
        foreach (XmlNode mediumNode in mediumNodes)
          {
            medium = loadMedium(mediumNode);
            mediums.AddLast(medium);
          }
      }

//     StreamReader fileStream = new StreamReader(@filePath);
// //     LinkedList<Medium> mediums;

//     string text = fileStream.ReadToEnd();
//     Debug.Log(text);
//     fileStream.Close();

    if (mediums.Count == 0)
      return null;
    return mediums;
  }
}