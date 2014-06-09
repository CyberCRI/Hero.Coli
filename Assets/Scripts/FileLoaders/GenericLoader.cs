using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;

public  class Loadable {

	public string id;
	
	public  void init(XmlNode node, string id)
	{
	}

}

public abstract class GenericLoader  {
	
	
	public enum attributesName{
		reactions,
		molecules
		
	}
	
	public LinkedList<T> loadObjectFromFiles<T> (string filePath, string tag)  
		where T : Loadable,  new()
			
	{
		//  Generics
		LinkedList<T> objectList;

		XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

		XmlNodeList objectNodeLists = xmlDoc.GetElementsByTagName(tag);
		
		
		objectList = specificLoader <T>(objectNodeLists);

		return objectList;

	}
	
	public abstract LinkedList<T> specificLoader<T> (XmlNodeList objectNodeLists)
		where T : Loadable, new();
	
	
}


