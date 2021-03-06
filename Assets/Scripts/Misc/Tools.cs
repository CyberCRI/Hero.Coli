using System;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*!
  \brief This class contains some useful functions
  
  
 */
class Tools
{
  /*!
    \brief Clean a string by removing space tab and return characters at its begining and at its end
    \param str The string to clean.
   */
  static public string epurStr(string str)
  {
    char[] charToStrip = {' ', '\t', '\n'};
    string strippedLine = str.TrimStart(charToStrip);
    strippedLine = strippedLine.TrimEnd(charToStrip);
    return strippedLine;
  }

  /*!
    \brief Get content of a file and encode it in UTF-8
    \param path The path of the file
   * /
  static public MemoryStream getEncodedFileContent(string path)
  {
    StreamReader fileStream = new StreamReader(@path);
    string text = fileStream.ReadToEnd();
    fileStream.Close();
    byte[] encodedString = Encoding.UTF8.GetBytes(text);
    MemoryStream ms = new MemoryStream(encodedString);
    ms.Flush();
    ms.Position = 0;

    return ms;
  }
  //*/


  /*!
    \brief Get content of a file in a Unity-export-compatible way
    \param filePath The path of the file
   */
  static public XmlDocument getXmlDocument(string filePath)
  {
    //Debug.LogError ("Tools getXmlDocument("+filePath+")");
    TextAsset temp = Resources.Load(filePath) as TextAsset;
    //string tempStr = (temp==null)?"(null)":temp.ToString();
    // Debug.Log(this.GetType() + " getXmlDocument "+tempStr);
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.LoadXml(temp.text);
    return xmlDoc;
  }

  public static void GetInterfaces<T>(out List<T> resultList, GameObject objectToSearch) where T: class {
    MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
    resultList = new List<T>();
    foreach(MonoBehaviour mb in list){
      if(mb is T){
        //found one
        resultList.Add((T)((System.Object)mb));
      }
    }
  }


    public static bool safeGetBool(string boolString)
    {
        if(string.IsNullOrEmpty(boolString))
        {
            Debug.LogWarning("Tools safeGetBool null string as bool");
            return false;
        }
        if(boolString.ToLower() != "false"
           && boolString.ToLower() != "true")
        {
            Debug.LogWarning("Tools safeGetBool unknwon string "+boolString+" as bool");
            return false;
        }
        
        return (boolString.ToLower() == "true");
    }
}