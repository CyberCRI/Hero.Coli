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
using System.Reflection;


public class FickLoader : XmlLoaderImpl
{
  private new string _xmlTag = "fickProp";

    
  //! Create from an XML node a FickProperties.
  //! \param node The XML node
  //! \return A FickProperties (descriptor of FickReaction)
  public FickProperties loadFickProperties(XmlNode node)
  {
    FickProperties props = new FickProperties();
    loadFickProperties(node, props);
    return props;
  }

  //! Create from an XML node a FickProperties.
  //! \param node The XML node
  //! \param properties The Fick properties that will be initialized by this loading.
  public void loadFickProperties(XmlNode node, FickProperties properties)
  {
    foreach (XmlNode attr in node)
    {
      switch (attr.Name)
      {
        case "MediumId1":
          properties.MediumId1 = Convert.ToInt32(attr.InnerText);
          break;
        case "MediumId2":
          properties.MediumId2 = Convert.ToInt32(attr.InnerText);
          break;
        case "P":
          properties.P = float.Parse(attr.InnerText.Replace(",", "."));
          break;
        case "surface":
          properties.surface = float.Parse(attr.InnerText.Replace(",", "."));
          break;
      }
    }
  }
}