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
        
        Debug.Log(this.GetType() + " loadObjectsFromFile with tag "+tag+" will load from "+filePath
                    );
		
		objectList = loadObjects <T>(objectNodeLists);
        
        Debug.Log(this.GetType() + " loadObjectsFromFile with tag "+tag+" loaded "+Logger.ToString<T>(objectList)+" from "+filePath
                    );

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
        Debug.Log(this.GetType() + " loadObjects with tag="+xmlTag+" will load"
                    );

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
            t.tryInstantiateFromXml(node);
            objectList.AddLast(t);
          }
        }
      }

        Debug.Log(this.GetType() + " loadObjects with tag "+xmlTag+" loaded "+Logger.ToString<T>(objectList)
                    );

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
            Debug.Log(tabulation + this.GetType() + " SpecificLog node="+node);
        }
    }
}


