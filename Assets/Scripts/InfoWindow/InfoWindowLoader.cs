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

  private StandardInfoWindowInfo _info;

  private void reinitVars() {
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
    Logger.Log("InfoWindowLoader::loadBioBricksFromFile("+filePath+")", Logger.Level.INFO);

    LinkedList<StandardInfoWindowInfo> resultInfo = new LinkedList<StandardInfoWindowInfo>(
      new List<StandardInfoWindowInfo>(){
        new StandardInfoWindowInfo("test2", "title2", "subtitle2", "tuto_intro-01", "explanation2", "bottom2")
      }
    );

    return resultInfo;
  }

}
