using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

public class AllosteryLoader
{
  private delegate void  StrSetter(string dst);
  private delegate void  FloatSetter(float dst);

  private bool loadAllosteryString(string value, StrSetter setter)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty name field");
        return false;
      }
    setter(value);
    return true;    
  }

  private bool loadAllosteryFloat(string value, FloatSetter setter)
  {
    if (String.IsNullOrEmpty(value))
      {
        Debug.Log("Error: Empty productionMax field");
        return false;
      }
    setter(float.Parse(value.Replace(",", ".")));
    return true;    
  }


  public bool loadAllostericReactions(XmlNode node, LinkedList<IReaction> reactions)
  {
    XmlNodeList AReactionsList = node.SelectNodes("allostery");
    bool b = true;
    
    foreach (XmlNode AReaction in AReactionsList)
      {
        Allostery ar = new Allostery();
        foreach (XmlNode attr in AReaction)
          {
            switch (attr.Name)
              {
              case "name":
                b = b && loadAllosteryString(attr.InnerText, ar.setName);
                break;
              case "effector":
                b = b && loadAllosteryString(attr.InnerText, ar.setEffector);
                break;
              case "K":
                b = b && loadAllosteryFloat(attr.InnerText, ar.setK);
                break;
              case "n":
                ar.setN(Convert.ToInt32(attr.InnerText));
                break;
              case "protein":
                b = b && loadAllosteryString(attr.InnerText, ar.setProtein);
                break;
              case "products":
                b = b && loadAllosteryString(attr.InnerText, ar.setProduct);
                break;
              }
          }
        reactions.AddLast(ar);
      }
    return b;
  }
}