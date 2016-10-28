using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;

/*!
  \brief This class loads all the files needed by the reaction Engine
  
 */

public class FileLoader : XmlLoader
{
  //! Default constructor
  public FileLoader()
  {
  }


  public override LinkedList<T> loadObjects<T> (XmlNodeList objectNodeList)
	{
		LinkedList<T> objectList = new LinkedList<T>();
		foreach (XmlNode objectNode in objectNodeList)
		{
			T t = new T();
      if(t.tryInstantiateFromXml(objectNode))
      {
        objectList.AddLast(t);
      }
      else
      {
        Debug.LogWarning(this.GetType() + " loadObjects incorrect data in "+Logger.ToString (objectNode));
      }  	
		}
		return objectList;
	}
}