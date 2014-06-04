using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;

public class Loadable {
	
	/*private string _tag;
	public static string getTag(){return _tag;}
	public static void setTag(string tag) {_tag = tag;}*/
	
	public static  void GLoad<T> (XmlNode node,string id, LinkedList<T> collection)
		where T : Loadable,  new()
	{
	}

	public Loadable()
	{
	}
	public Loadable (XmlNode node, string id)
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
		LinkedList<T> objectList = new LinkedList<T>();

		XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

		XmlNodeList objectNodeLists = xmlDoc.GetElementsByTagName(tag);
		
		
		specificLoader <T>(objectList, objectNodeLists);
		
		/*var converter = TypeDescriptor.GetConverter(oftype);
		var result = converter.ConvertFrom(objectList);*/
		Logger.Log ("GenericLoader::objectlist Size::"+objectList.Count,Logger.Level.WARN);
		return objectList;

	}
	
	public abstract void specificLoader<T> (LinkedList<T> objectList, XmlNodeList objectNodeLists)
		where T : Loadable, new();
	
	
}


