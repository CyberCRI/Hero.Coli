using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;

public abstract class XmlLoader  {
	
	public LinkedList<T> loadObjectsFromFile<T> (string filePath, string tag)  
		where T : XmlLoadable,  new()
			
	{
		LinkedList<T> objectList;

		XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

		XmlNodeList objectNodeLists = xmlDoc.GetElementsByTagName(tag);
		
		
		objectList = loadObjects <T>(objectNodeLists);

		return objectList;

	}
	
  public abstract LinkedList<T> loadObjects<T> (XmlNodeList objectNodeLists)
		where T : XmlLoadable, new();
}


