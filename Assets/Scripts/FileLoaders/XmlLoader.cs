using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;

public abstract class XmlLoader
{
	
	public LinkedList<T> loadObjectsFromFile<T> (string filePath, string tag)  
		where T : LoadableFromXml,  new()
			
	{
		LinkedList<T> objectList;

		XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

		XmlNodeList objectNodeLists = xmlDoc.GetElementsByTagName(tag);
        
        Logger.Log ("XmlLoader::loadObjectsFromFile with tag "+tag+" will load from "+filePath
                    , Logger.Level.DEBUG);
		
		objectList = loadObjects <T>(objectNodeLists);
        
        Logger.Log ("XmlLoader::loadObjectsFromFile with tag "+tag+" loaded "+Logger.ToString<T>(objectList)+" from "+filePath
                    , Logger.Level.DEBUG);

		return objectList;

	}
	
  public abstract LinkedList<T> loadObjects<T> (XmlNodeList objectNodeLists)
		where T : LoadableFromXml, new();
}

public abstract class XmlLoaderImpl : XmlLoader
{ 
  //property
  public abstract string xmlTag
  {
    get;
  }

  public override LinkedList<T> loadObjects<T> (XmlNodeList objectNodeLists)
      //where T : LoadableFromXml, new()
  {
        Logger.Log ("XmlLoaderImpl::loadObjects with tag="+xmlTag+" will load"
                    , Logger.Level.DEBUG);

      LinkedList<T> objectList = new LinkedList<T>();

      foreach (XmlNode nodes in objectNodeLists)
      {
        SpecificLog(nodes);
        foreach (XmlNode node in nodes)
        {
          SpecificLog(node, 1);
          if (node.Name == xmlTag)
          {
            T t = new T();
            t.initFromLoad(node, this);
            objectList.AddLast(t);
          }
        }
      }

        Logger.Log ("XmlLoaderImpl::loadObjects with tag "+xmlTag+" loaded "+Logger.ToString<T>(objectList)
                    , Logger.Level.DEBUG);

      if (objectList.Count == 0)
        return null;
      return objectList;
  }

    //TODO remove (debug function)
    private void SpecificLog(XmlNode node, int level = 0)
    {
        if(xmlTag == "molecules")
        {
            string tabulation = new string(' ', 4*level);
            Logger.Log(tabulation+ "XmlLoaderImpl::SpecificLog node="+node, Logger.Level.DEBUG);
        }
    }
}


