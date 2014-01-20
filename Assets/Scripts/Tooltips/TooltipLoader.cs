using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;

//TODO refactor with FileLoader
public class TooltipLoader {

  private string _code;
  private string _title;
  private string _subtitle;
  private string _texture;
  private string _explanation;
  private string _bottom;

  private TooltipInfo _info;

  private void reinitVars() {
    _code = null;
    _title = null;
    _subtitle = null;
    _texture = null;
    _explanation = null;
    _bottom = null;
    _info = null;
  }

  public LinkedList<TooltipInfo> loadInfoFromFile(string filePath)
  {
    Logger.Log("::loadInfoFromFile("+filePath+")", Logger.Level.INFO);

    LinkedList<TooltipInfo> resultInfo = new LinkedList<TooltipInfo>();

    XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

    XmlNodeList infoList = xmlDoc.GetElementsByTagName(TooltipXMLTags.TOOLTIP);

    foreach (XmlNode infoNode in infoList)
    {
      reinitVars();
      //common info attributes
      try {
        _code = infoNode.Attributes[TooltipXMLTags.CODE].Value;
      }
      catch (NullReferenceException exc) {
        Logger.Log("TooltipLoader::loadInfoFromFile bad xml, missing field\n"+exc, Logger.Level.WARN);
        continue;
      }
      catch (Exception exc) {
        Logger.Log("TooltipLoader::loadInfoFromFile failed, got exc="+exc, Logger.Level.WARN);
        continue;
      }

      if (checkString(_code)) {
        foreach (XmlNode attr in infoNode){
          switch (attr.Name){
            case TooltipXMLTags.TITLE:
              _title = attr.InnerText;
              break;
            case TooltipXMLTags.SUBTITLE:
              _subtitle = attr.InnerText;
              break;
            case TooltipXMLTags.TEXTURE:
              _texture = attr.InnerText;
              break;
            case TooltipXMLTags.EXPLANATION:
              _explanation = attr.InnerText;
              break;
            case TooltipXMLTags.BOTTOM:
              _bottom = attr.InnerText;
              break;
            default:
                Logger.Log("TooltipLoader::loadInfoFromFile unknown attr "+attr.Name+" for info node", Logger.Level.WARN);
              break;
          }
        }
        if(
          checkString(_title)
          && checkString(_subtitle)
          && checkString(_texture)
          && checkString(_explanation)
          && checkString(_bottom)
          )
        {
          _info = new TooltipInfo(_code, _title, _subtitle, _texture, _explanation, _bottom);
        }
        if(null != _info)
        {
          resultInfo.AddLast(_info);
        }
      }
      else
      {
        Logger.Log("TooltipLoader::loadInfoFromFile Error : missing attribute code in info node", Logger.Level.WARN);
      }
    }
    return resultInfo;
  }

  private bool checkString(string toCheck) {
    return !String.IsNullOrEmpty(toCheck);
  }

}
