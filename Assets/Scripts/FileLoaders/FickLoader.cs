using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System;
using System.IO;


/*!
 *  \brief     Load fick reaction files
 *  \details   Load files that describe FickReaction.
 A fick reaction file should respect this synthax :
 
     <ficks>
      <fickProp>
        <MediumId1>2</MediumId1>                -> Unique ID of the first medium
        <MediumId2>1</MediumId2>                -> Unique ID of the second medium
        <P>0.05</P>                             -> Permeability coefficient
        <surface>0.003</surface>                -> contact surface between the two medium
      </fickProp>
     </ficks>

 *  \author    Pierre COLLET
 *  \mail      pierre.collet91@gmail.com
 */
public class FickLoader
{
  //! Create from an XML node a FickProprieties.
  //! \param node The XML node
  //! \return A FickProprieties (descriptor of FickReaction)
  public FickProprieties   loadFickProprieties(XmlNode node)
  {
    FickProprieties props = new FickProprieties();

    foreach (XmlNode attr in node)
      {
        switch (attr.Name)
          {
          case "MediumId1":
            props.MediumId1 = Convert.ToInt32(attr.InnerText);
            break;
          case "MediumId2":
            props.MediumId2 = Convert.ToInt32(attr.InnerText);
            break;
          case "P":
            props.P = float.Parse(attr.InnerText.Replace(",", "."));
            break;
          case "surface":
            props.surface = float.Parse(attr.InnerText.Replace(",", "."));
            break;
          }
      }
    return props;
  }

  //! Create a LinkedList of FickProprieties from a file path.
  //! \param filePath the path to the file
  //! \return A list of all FickReaction descriptor (FickProprieties) present in a file
  public LinkedList<FickProprieties>     loadFickProprietiesFromFile(string filePath)
  {
    LinkedList<FickProprieties> ficksProps = new LinkedList<FickProprieties>();
    FickProprieties prop;

    MemoryStream ms = Tools.getEncodedFileContent(filePath);
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.Load(ms);

    XmlNodeList ficksLists = xmlDoc.GetElementsByTagName("ficks");
    foreach (XmlNode ficksNodes in ficksLists)
      {
        foreach (XmlNode fickNode in ficksNodes)
          {
            if (fickNode.Name == "fickProp")
              {
                prop = loadFickProprieties(fickNode);
                ficksProps.AddLast(prop);
              }
          }
      }

    if (ficksProps.Count == 0)
      return null;
    return ficksProps;
  }
}