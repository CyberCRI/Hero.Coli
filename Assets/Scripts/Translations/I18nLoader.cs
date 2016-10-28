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
    Debug.Log(this.GetType() + " I18nLoader::I18nLoader("+dicos+")");
    _dicos = dicos;
  }


  public void loadFromFile(string filePath, I18n.Language lang)
  {
    Debug.Log(this.GetType() + " I18nLoader::loadFromFile("+filePath+")");

    XmlDocument xmlDoc = Tools.getXmlDocument(filePath);
    
    XmlNodeList translations = xmlDoc.GetElementsByTagName(I18nXMLTags.ITEM);

    reinitVars();

    foreach (XmlNode translation in translations)
    {
      try {
        _code = translation.Attributes[I18nXMLTags.CODE].Value;
      }
      catch (NullReferenceException exc) {
        Debug.LogWarning(this.GetType() + " loadFromFile bad xml, missing field \""+I18nXMLTags.CODE+"\"\n"+exc);
        continue;
      }
      catch (Exception exc) {
        Debug.LogWarning(this.GetType() + " loadFromFile failed, got exc="+exc);
        continue;
      }

      Debug.Log(this.GetType() + " I18nLoader::loadFromFile got "+I18nXMLTags.CODE+"="+_code
        );

      if (checkString(_code))
      {
        try {
          _translation = translation.Attributes[I18nXMLTags.TRANSLATION].Value;
        }
        catch (NullReferenceException exc) {
          Debug.LogWarning(this.GetType() + " loadFromFile bad xml, missing field \""+I18nXMLTags.TRANSLATION+"\"\n"+exc);
          continue;
        }
        catch (Exception exc) {
          Debug.LogWarning(this.GetType() + " loadFromFile failed, got exc="+exc);
          continue;
        }

        if(checkString(_translation))
        {
          Debug.Log(this.GetType() + " I18nLoader::loadFromFile adding "+I18nXMLTags.TRANSLATION
          +"="+_translation+" for \""+I18nXMLTags.CODE+"\"="+_code+"\n"
          );
          _dicos[lang].Add(_code, _translation);
        }
        else
        {
          Debug.LogWarning(this.GetType() + " loadFromFile bad xml, missing "+I18nXMLTags.TRANSLATION+" for \""+I18nXMLTags.CODE+"\"="+_code+"\n");
        }
      } else {
        Debug.LogWarning(this.GetType() + " loadFromFile Error : missing attribute "+I18nXMLTags.CODE+" in translation node");
      }
      reinitVars();
    }
  }

  private bool checkString(string toCheck) {
    return !String.IsNullOrEmpty(toCheck);
  }

}
