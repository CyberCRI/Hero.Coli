using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;

//TODO refactor with FileLoader
public class InfoWindowLoader {

  private string _code;
  private string _texture;
  private string _next;
  private string _cancel;

  private StandardInfoWindowInfo _info;

  private void reinitVars() {
    _code = null;
    _texture = null;
    _next = null;
    _cancel = null;
        
    _info = null;
  }

  public LinkedList<StandardInfoWindowInfo> loadInfoFromFile(string filePath)
  {
    Logger.Log("InfoWindowLoader::loadInfoFromFile("+filePath+")", Logger.Level.INFO);

    LinkedList<StandardInfoWindowInfo> resultInfo = new LinkedList<StandardInfoWindowInfo>();

    XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

    XmlNodeList infoList = xmlDoc.GetElementsByTagName(InfoWindowXMLTags.INFO);

    foreach (XmlNode infoNode in infoList)
    {
      reinitVars();
      //common info attributes
      try {
        _code = infoNode.Attributes[InfoWindowXMLTags.CODE].Value;
      }
      catch (NullReferenceException exc) {
        Debug.LogWarning(this.GetType() + " loadInfoFromFile bad xml, missing field\n"+exc);
        continue;
      }
      catch (Exception exc) {
        Debug.LogWarning(this.GetType() + " loadInfoFromFile failed, got exc="+exc);
        continue;
      }

      if (checkString(_code)) {
        foreach (XmlNode attr in infoNode){
          switch (attr.Name){
            case InfoWindowXMLTags.TEXTURE:
              _texture = attr.InnerText;
              break;
            case InfoWindowXMLTags.NEXT:
              _next = attr.InnerText;
                break;
            case InfoWindowXMLTags.CANCEL:
              _cancel = attr.InnerText;
              break;
            default:
                Debug.LogWarning(this.GetType() + " loadInfoFromFile unknown attr "+attr.Name+" for info node");
              break;
          }
        }
        if(
             checkString(_texture)
          && checkString(_next)
          //_cancel is optional, therefore no need to check it
          )
        {
          _info = new StandardInfoWindowInfo(_code, _texture, _next, _cancel);
        }
        if(null != _info)
        {
          resultInfo.AddLast(_info);
        }
      }
      else
      {
        Debug.LogWarning(this.GetType() + " loadInfoFromFile Error : missing attribute code in info node");
      }
    }
    return resultInfo;
  }

  private bool checkString(string toCheck) {
    return !String.IsNullOrEmpty(toCheck);
  }

}
