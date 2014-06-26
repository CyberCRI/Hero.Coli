using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;

/*!
\brief This class load all the allostery reactions
\details A allostery's reaction's declaration should respect this synthax :

    <allostery>
      <name>inhibitLacI</name>
      <effector>IPTG</effector>
      <EnergyCost>0.3</EnergyCost>
      <K>0.1</K>
      <n>2</n>
      <protein>LacI</protein>
      <products>LacI*</products>
    </allostery>

\sa Allostery

*/
public class AllosteryLoader
{
  private delegate void  StrSetter(string dst);
  private delegate void  FloatSetter(float dst);

  /*!
\brief This function load and parse a string and give it to the given setter
\param value The string to parse and load
\param setter The delegate setter
\return Return true is success false otherwise
  */
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

  /*!
\brief This function load and parse a string and give it to the given setter
\param value The string to parse and load
\param setter The delegate setter
\return Return true is success false otherwise
  */
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


  /*!
\brief This function load all the allostric reactions and add them to the given IReaction list
\param node The xml load to parse
\param reactions The list of reaction where will be appened the new allosteric reactions
\return Return true of succed and false if the function has failed
  */
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
              case "EnergyCost":
                b = b && loadAllosteryFloat(attr.InnerText, ar.setEnergyCost);
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