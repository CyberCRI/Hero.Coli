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


public class FickLoader : GenericLoader
{
  //! Create from an XML node a FickProperties.
  //! \param node The XML node
  //! \return A FickProperties (descriptor of FickReaction)
  public FickProperties   loadFickProperties(XmlNode node)
  {
    FickProperties props = new FickProperties();

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

	public override LinkedList<T> loadFromXml<T> (XmlNodeList ficksLists)
	{
		LinkedList<T> objectList = new LinkedList<T>();
		T t = new T();
		FickLoader loader = new FickLoader();


		// Reflection Call
		MethodInfo method = typeof(T).GetMethod("initFromLoad");
		object[] mParam;

		foreach (XmlNode ficksNodes in ficksLists)
		{
			foreach (XmlNode fickNode in ficksNodes)
			{
				if (fickNode.Name == "fickProp")
				{
					mParam = new object[] {fickNode, loader};
					t =(T) method.Invoke (t,mParam);
					objectList.AddLast(t);
				}
			}
		}
		
		if (objectList.Count == 0)
			return null;
		return objectList;
	}
}