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
  private string _type;
  private string _subtitle;
  private string _texture;
  private string _length;
  private string _reference;
  private string _explanation;

  private TooltipInfo _info;

  private void reinitVars() {
    _code = null;
    _title = null;
    _type = null;
    _subtitle = null;
    _texture = null;
    _length = null;
    _reference = null;
    _explanation = null;
    _info = null;
  }

  public LinkedList<TooltipInfo> loadInfoFromFile(string filePath)
  {
    Logger.Log("TooltipLoader::loadInfoFromFile("+filePath+")", Logger.Level.TEMP);

    LinkedList<TooltipInfo> resultInfo = new LinkedList<TooltipInfo>();

    XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

    XmlNodeList infoList = xmlDoc.GetElementsByTagName(TooltipXMLTags.TOOLTIP);

    foreach (XmlNode infoNode in infoList)
    {
      Logger.Log("TooltipLoader::loadInfoFromFile("+filePath+") infoNode="+infoNode, Logger.Level.TEMP);
      reinitVars();
      //common info attributes
      try {
        _code = infoNode.Attributes[TooltipXMLTags.CODE].Value;
        Logger.Log("TooltipLoader::loadInfoFromFile("+filePath+") _code="+_code, Logger.Level.TEMP);
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
            case TooltipXMLTags.TYPE:
              _type = attr.InnerText;
              break;
            case TooltipXMLTags.SUBTITLE:
              _subtitle = attr.InnerText;
              break;
            case TooltipXMLTags.TEXTURE:
              _texture = attr.InnerText;
              break;
            case TooltipXMLTags.LENGTH:
              _length = attr.InnerText;
              break;
            case TooltipXMLTags.REFERENCE:
              _reference = attr.InnerText;
              break;
            case TooltipXMLTags.EXPLANATION:
              _explanation = attr.InnerText;
              break;
            default:
                Logger.Log("TooltipLoader::loadInfoFromFile unknown attr "+attr.Name+" for info node", Logger.Level.WARN);
              break;
          }
        }
        if(
          checkString(_title)
          && checkString(_type)
          && checkString(_subtitle)
          && checkString(_texture)
          && checkString(_length)
          && checkString(_reference)
          && checkString(_explanation)
          )
        {
          _info = new TooltipInfo(_code, _title, _type, _subtitle, _texture, _length, _reference, _explanation);
        }
        if(null != _info)
        {
          Logger.Log("TooltipLoader::loadInfoFromFile("+filePath+") AddLast(_info)="+_info, Logger.Level.TEMP);
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
