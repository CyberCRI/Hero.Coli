using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;

//TODO refactor with FileLoader
public class I18nLoader {
  private Dictionary<I18n.Language, IDictionary<string, string>> _dicos;
  private string _code;
  private string _translation;

  private void reinitVars() {
    _code = null;
    _translation = null;
  }

  public I18nLoader(Dictionary<I18n.Language, IDictionary<string, string>> dicos) {
    Logger.Log("I18nLoader::I18nLoader("+dicos+")", Logger.Level.DEBUG);
    _dicos = dicos;
  }


  public void loadFromFile(string filePath, I18n.Language lang)
  {
    Logger.Log("I18nLoader::loadFromFile("+filePath+")", Logger.Level.INFO);

    MemoryStream ms = Tools.getEncodedFileContent(filePath);

    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.Load(ms);
    XmlNodeList translations = xmlDoc.GetElementsByTagName(I18nXMLTags.ITEM);

    reinitVars();

    foreach (XmlNode translation in translations)
    {
      try {
        _code = translation.Attributes[I18nXMLTags.CODE].Value;
      }
      catch (NullReferenceException exc) {
        Logger.Log("I18nLoader::loadFromFile bad xml, missing field \""+I18nXMLTags.CODE+"\"\n"+exc, Logger.Level.WARN);
        continue;
      }
      catch (Exception exc) {
        Logger.Log("I18nLoader::loadFromFile failed, got exc="+exc, Logger.Level.WARN);
        continue;
      }

      Logger.Log ("I18nLoader::loadFromFile got "+I18nXMLTags.CODE+"="+_code
        , Logger.Level.TRACE);

      if (checkString(_code))
      {
        try {
          _translation = translation.Attributes[I18nXMLTags.TRANSLATION].Value;
        }
        catch (NullReferenceException exc) {
          Logger.Log("I18nLoader::loadFromFile bad xml, missing field \""+I18nXMLTags.TRANSLATION+"\"\n"+exc, Logger.Level.WARN);
          continue;
        }
        catch (Exception exc) {
          Logger.Log("I18nLoader::loadFromFile failed, got exc="+exc, Logger.Level.WARN);
          continue;
        }

        if(checkString(_translation))
        {
          Logger.Log("I18nLoader::loadFromFile adding "+I18nXMLTags.TRANSLATION
          +"="+_translation+" for \""+I18nXMLTags.CODE+"\"="+_code+"\n"
          , Logger.Level.TRACE);
          _dicos[lang].Add(_code, _translation);
        }
        else
        {
          Logger.Log("I18nLoader::loadFromFile bad xml, missing "+I18nXMLTags.TRANSLATION
          +" for \""+I18nXMLTags.CODE+"\"="+_code+"\n"
          , Logger.Level.WARN);
        }
      } else {
        Logger.Log("I18nLoader::loadFromFile Error : missing attribute "+I18nXMLTags.CODE+" in translation node", Logger.Level.WARN);
      }
      reinitVars();
    }
  }

  private bool checkString(string toCheck) {
    return !String.IsNullOrEmpty(toCheck);
  }

}
