using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;

public abstract class GenericLoader  {
	
	
	public enum attributesName{
		reactions,
		molecules
		
	}
	
	public LinkedList<T> loadObjectFromFile<T> (string filePath, string tag)  
		where T : XMLLoadable,  new()
			
	{
		//  Generics
		LinkedList<T> objectList;

		XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

		XmlNodeList objectNodeLists = xmlDoc.GetElementsByTagName(tag);
		
		
		objectList = loadFromXml <T>(objectNodeLists);

		return objectList;

	}
	
	public abstract LinkedList<T> loadFromXml<T> (XmlNodeList objectNodeLists)
		where T : XMLLoadable, new();
	
	
}


