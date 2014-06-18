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
            <ReactionSet>CelliaReactions</ReactionSet>        -> ReactionSet to load in the medium
            <MoleculeSet>CelliaMolecules</MoleculeSet>        -> MoleculeSet to load in the medium
            <Energy>1000</Energy>                               -> Initial Energy
            <MaxEnergy>2000</MaxEnergy>                         -> Maximal energy
            <EnergyProductionRate>10</EnergyProductionRate>     -> The energy production speed
          </Medium>
        </Mediums>

 *  \author    Pierre COLLET
 *  \sa ReactionSet
 *  \sa MoleculeSet
 *  \sa Medium
 */
using System.Reflection;


public class MediumLoader : GenericLoader
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
  public Medium loadMedium(XmlNode node)
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
          case "ReactionSet":
            medium.setReactionSet(attr.InnerText);
            break;
          case "MoleculeSet":
            medium.setMoleculeSet(attr.InnerText);
            break;
          }
      }
    return medium;
  }

	public override LinkedList<T> loadFromXml<T> (XmlNodeList objectNodeLists)
	{
		LinkedList<T> objectList = new LinkedList<T>();
		T t = new T();
		MediumLoader loader = new MediumLoader();

		// Reflection Call
		MethodInfo method = typeof(T).GetMethod("initFromLoad");
		object[] mParam;



		foreach (XmlNode mediumNodes in objectNodeLists)
		{
			foreach (XmlNode mediumNode in mediumNodes)
			{
				mParam = new object[] {mediumNode,loader};
				t =(T) method.Invoke(t,mParam);
				objectList.AddLast(t);
			}
		}
		return objectList;
	}
}