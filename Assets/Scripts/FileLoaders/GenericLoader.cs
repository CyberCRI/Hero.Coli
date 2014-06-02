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
	
	public static  void GLoad ()
	{
	}
	
	
}

public abstract class GenericLoader  {
	
	
	public enum attributesName{
		reactions,
		molecules
		
	}
	
	public R loadObjectFromFiles<T,R> (string filePath, string tag)  
		where T : Loadable,  new()
		where R : ICollection<T>, new ()
			
	{
		//  Generics
		R objectList = new R();

		XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

		XmlNodeList objectNodeLists = xmlDoc.GetElementsByTagName(tag);
		
		
		specificLoader <T,R>(objectList, objectNodeLists, tag);
		
		/*var converter = TypeDescriptor.GetConverter(oftype);
		var result = converter.ConvertFrom(objectList);*/

		return objectList;

	}
	
	public abstract void specificLoader<T,R> (R objectList, XmlNodeList objectNodeLists, string tag)
		where T : Loadable, new()
		where R : ICollection<T>, new ();
	
	
}


