using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;

//TODO refactor with FileLoader
public class InfoWindowLoader {

  private string _code;
  private string _title;
  private string _subtitle;
  private string _texture;
  private string _explanation;
  private string _bottom;
  private string _next;

  private StandardInfoWindowInfo _info;

  private void reinitVars() {
    Logger.Log ("InfoWindowLoader::reinitVars", Logger.Level.TEMP);
    _code = null;
    _title = null;
    _subtitle = null;
    _texture = null;
    _explanation = null;
    _bottom = null;
    _info = null;
  }

  public InfoWindowLoader() {
    Logger.Log("InfoWindowLoader::InfoWindowLoader()");
  }


  public LinkedList<StandardInfoWindowInfo> loadInfoFromFile(string filePath)
  {
    Logger.Log("InfoWindowLoader::loadInfoFromFile("+filePath+")", Logger.Level.INFO);

    LinkedList<StandardInfoWindowInfo> resultInfo = new LinkedList<StandardInfoWindowInfo>();

    Logger.Log ("InfoWindowLoader::loadInfoFromFile got getXmlDocument", Logger.Level.TEMP);
    XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

    Logger.Log ("InfoWindowLoader::loadInfoFromFile GetElementsByTagName", Logger.Level.TEMP);
    XmlNodeList infoList = xmlDoc.GetElementsByTagName(InfoWindowXMLTags.INFO);

    foreach (XmlNode infoNode in infoList)
    {
      reinitVars();
      //common info attributes
      try {
        Logger.Log ("InfoWindowLoader::loadInfoFromFile trying to get code", Logger.Level.TEMP);
        _code = infoNode.Attributes[InfoWindowXMLTags.CODE].Value;
      }
      catch (NullReferenceException exc) {
        Logger.Log("InfoWindowLoader::loadInfoFromFile bad xml, missing field\n"+exc, Logger.Level.WARN);
        continue;
      }
      catch (Exception exc) {
        Logger.Log("InfoWindowLoader::loadInfoFromFile failed, got exc="+exc, Logger.Level.WARN);
        continue;
      }
      Logger.Log ("InfoWindowLoader::loadInfoFromFile got code="+_code, Logger.Level.TEMP);

      if (checkString(_code)) {
        foreach (XmlNode attr in infoNode){
          switch (attr.Name){
            case InfoWindowXMLTags.TITLE:
              _title = attr.InnerText;
              Logger.Log("InfoWindowLoader::loadInfoFromFile got _title="+_title, Logger.Level.TEMP);
              break;
            case InfoWindowXMLTags.SUBTITLE:
              _subtitle = attr.InnerText;
              Logger.Log("InfoWindowLoader::loadInfoFromFile got _subtitle="+_subtitle, Logger.Level.TEMP);
              break;
            case InfoWindowXMLTags.TEXTURE:
              _texture = attr.InnerText;
              Logger.Log("InfoWindowLoader::loadInfoFromFile got _texture="+_texture, Logger.Level.TEMP);
              break;
            case InfoWindowXMLTags.EXPLANATION:
              _explanation = attr.InnerText;
              Logger.Log("InfoWindowLoader::loadInfoFromFile got _explanation="+_explanation, Logger.Level.TEMP);
              break;
            case InfoWindowXMLTags.BOTTOM:
              _bottom = attr.InnerText;
              Logger.Log("InfoWindowLoader::loadInfoFromFile got _bottom="+_bottom, Logger.Level.TEMP);
              break;
            case InfoWindowXMLTags.NEXT:
              _next = attr.InnerText;
              Logger.Log("InfoWindowLoader::loadInfoFromFile got _next="+_next, Logger.Level.TEMP);
              break;
            default:
                Logger.Log("InfoWindowLoader::loadInfoFromFile unknown attr "+attr.Name+" for info node", Logger.Level.WARN);
              break;
          }
        }
        if(
          checkString(_title)
          && checkString(_subtitle)
          && checkString(_texture)
          && checkString(_explanation)
          && checkString(_bottom)
          && checkString(_next)
          )
        {
          Logger.Log("InfoWindowLoader::loadInfoFromFile instanciating new _info", Logger.Level.TEMP);
          _info = new StandardInfoWindowInfo(_code, _title, _subtitle, _texture, _explanation, _bottom, _next);
        }
        if(null != _info)
        {
          Logger.Log("InfoWindowLoader::loadInfoFromFile adding to list new _info="+_info, Logger.Level.TEMP);
          resultInfo.AddLast(_info);
        }
      }
      else
      {
        Logger.Log("InfoWindowLoader::loadInfoFromFile Error : missing attribute code in info node", Logger.Level.WARN);
      }
    }
    return resultInfo;
  }

  private bool checkString(string toCheck) {
    return !String.IsNullOrEmpty(toCheck);
  }

}
