using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Linq;

public class Misc
{
  public bool CheckDefault<T>(T value)
  {
    return EqualityComparer<T>.Default.Equals(value, default(T));
  }
}

public class LevelScript : MonoBehaviour
{
  //To be edited to add new blocks.
  public enum CellType
  {
    EMPTY = 0,
    WALL = 1,
    START = 2,
    TOXIC = 3,
    AIRFLOWUP = 4,
    AIRFLOWDOWN = 5,
    ROCK = 6,
    UNKNOWN = 7
  }

  public struct Level
  {
    public int          id;
    public string       name;
    public string       description;
    public CellType[]  map;
    public int[]       mediumMap;
    public Int32       sizeX;
    public Int32       sizeY;
  }

  private readonly char[]   MapSeparators = {' ', '\t'};
  private readonly char[]   MapToStripChar = {' ', '\t', '\n', '\r'};
  
  private List<Level>   _levels {get; set;}

  //To be edited to add new blocks.
  public TextAsset      _levelsPath;
  public GameObject     _groundCell;
  public GameObject     _wallCell;
  public GameObject     _toxicCell;
  public GameObject     _airFlowUpCell;
  public GameObject     _airFlowDownCell;
  public GameObject     _rockCell;

  private void  setMapSizeAttrs(ref Level level, string map)
  {
    String[]    lines;
    String[]    cols;
    String      strippedLine;
    Int32      i = 0;
    Int32      j;

    level.sizeX = 0;
    lines = map.Split('\n');
    foreach (string l in lines)
      {
        strippedLine = l.TrimStart(MapToStripChar);
        strippedLine = strippedLine.TrimEnd(MapToStripChar);
        cols = strippedLine.Split(MapSeparators);
        j = cols.Length;
        if (level.sizeX < j)
          level.sizeX = j;
        i++;
      }
    level.sizeY = i;
  }

  private void  convertStringMapToCellType(ref Level level, string map)
  {
    String[]    lines;
    String[]    cols;
    String      strippedLine;
    Int32      i = 0;
    Int32      j;

    level.map = Enumerable.Repeat(CellType.EMPTY, level.sizeY * level.sizeX).ToArray();
    level.map = new CellType[level.sizeY * level.sizeX];
    lines = map.Split('\n');
    foreach (string l in lines)
      {
        strippedLine = l.TrimStart(MapToStripChar);
        strippedLine = strippedLine.TrimEnd(MapToStripChar);
        cols = strippedLine.Split(MapSeparators);
        j = 0;
        foreach (string c in cols)
          {
            //To be edited to add new blocks.
            switch (Convert.ToInt32(c))
              {
              case (int)CellType.EMPTY:
                level.map[level.sizeX * i + j] = CellType.EMPTY;
                break;
              case (int)CellType.WALL:
                level.map[level.sizeX * i + j] = CellType.WALL;
                break;
              case (int)CellType.START:
                level.map[level.sizeX * i + j] = CellType.START;
                break;
              case (int)CellType.TOXIC:
                level.map[level.sizeX * i + j] = CellType.TOXIC;
                break;
              case (int)CellType.AIRFLOWUP:
                level.map[level.sizeX * i + j] = CellType.AIRFLOWUP;
                break;
              case (int)CellType.AIRFLOWDOWN:
                level.map[level.sizeX * i + j] = CellType.AIRFLOWDOWN;
                break;
              case (int)CellType.ROCK:
                level.map[level.sizeX * i + j] = CellType.ROCK;
                break;
              default:
                level.map[level.sizeX * i + j] = CellType.UNKNOWN;
                break;
              }
            j++;
          }
        i++;                        
      }
  }


  private void  convertStringMapToInt(ref Level level, string map)
  {
    String[]    lines;
    String[]    cols;
    String      strippedLine;
    Int32      i = 0;
    Int32      j;

    level.mediumMap = Enumerable.Repeat(0, level.sizeY * level.sizeX).ToArray();
    level.mediumMap = new int[level.sizeY * level.sizeX];
    lines = map.Split('\n');
    foreach (string l in lines)
      {
        strippedLine = l.TrimStart(MapToStripChar);
        strippedLine = strippedLine.TrimEnd(MapToStripChar);
        cols = strippedLine.Split(MapSeparators);
        j = 0;
        foreach (string c in cols)
          {
            level.mediumMap[level.sizeX * i + j] = Convert.ToInt32(c);
            j++;
          }
        i++;                        
      }
  }


  private void  setMapAttrs(ref Level level, string map)
  {
    String strippedLine;

    strippedLine = map.TrimStart(MapToStripChar);
    strippedLine = strippedLine.TrimEnd(MapToStripChar);
    
    setMapSizeAttrs(ref level, strippedLine);
    convertStringMapToCellType(ref level, strippedLine);
  }

  private void  setMediumMapAttrs(ref Level level, string map)
  {
    String strippedLine;

    strippedLine = map.TrimStart(MapToStripChar);
    strippedLine = strippedLine.TrimEnd(MapToStripChar);
    
    setMapSizeAttrs(ref level, strippedLine);
    convertStringMapToInt(ref level, strippedLine);
  }
  
  private void  loadLevelsFromFile(TextAsset path)
  {
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.LoadXml(path.text);
    XmlNodeList levelsLists = xmlDoc.GetElementsByTagName("levels");

    foreach (XmlNode levelsList in levelsLists)
      {
        XmlNodeList levelLists = levelsList.SelectNodes("level");
        foreach (XmlNode levelNode in levelLists)
          {
            Level l = new Level();
            l.id = 0;
            l.name = "";
            l.description = "";
            l.map = new CellType[0];
            l.sizeX = 0;
            l.sizeY = 0;
            foreach (XmlNode item in levelNode)
              {
                if (item.Name == "id")
                  l.id = Convert.ToInt32(item.InnerText);
                else if (item.Name == "name")
                  l.name = item.InnerText;
                else if (item.Name == "description")
                  l.description = item.InnerText;
                else if (item.Name == "map")
                  setMapAttrs(ref l, item.InnerText);
                else if (item.Name == "mediumMap")
                  setMediumMapAttrs(ref l, item.InnerText);
              }
            _levels.Add(l);
          }
      }
  }

  private void  createCube(Int32 x, Int32 y, Int32 z, Int32 scale, GameObject cell)
  {
    Vector3 aPosition = new Vector3(x, y, z);
    Quaternion rot = Quaternion.identity;
    rot = Quaternion.Euler(0, 0, 0);
    Instantiate(cell, aPosition, rot);
  }

  private void  drawMap(Level level)
  {
    int i;

    for (i = 0; i < level.map.Length; i++)
      {
        //To be edited to add new blocks.
        if (level.map[i] == CellType.WALL)
          createCube(i % level.sizeX + 1, 0, level.sizeY - i / level.sizeX, 1, _wallCell);
        //else if (level.map[i] == CellType.EMPTY)
        //  createCube(i % level.sizeX + 1, 0, i / level.sizeX,  1, _groundCell);
        //else if (level.map[i] == CellType.START)
        //  createCube(i % level.sizeX + 1, 0, i / level.sizeX, 1, _groundCell);
        
        else if (level.map[i] == CellType.TOXIC)
          createCube(i % level.sizeX + 1, 0,  level.sizeY - i / level.sizeX, 1, _toxicCell);
        else if (level.map[i] == CellType.AIRFLOWUP)
          createCube(i % level.sizeX + 1, 0,  level.sizeY - i / level.sizeX, 1, _airFlowUpCell);
        else if (level.map[i] == CellType.AIRFLOWDOWN)
          createCube(i % level.sizeX + 1, 0,  level.sizeY - i / level.sizeX, 1, _airFlowDownCell);
        else if (level.map[i] == CellType.ROCK)
          createCube(i % level.sizeX + 1, 0,  level.sizeY - i / level.sizeX, 1, _rockCell);
      }
    
  }

  private Level   getLevelFromId(Int32 id)
  {
    return _levels.Find(delegate (Level l) {return l.id == id;});
  }

  private bool  LoadLevel(Int32 id)
  {
    Level level = getLevelFromId(id);
    if (EqualityComparer<Level>.Default.Equals(level, default(Level)))
      {
        Debug.Log("Failed to load level with id: "+id);
        return false;
      }
    Debug.Log("Level " + level.name + " loaded.");

    drawMap(level);

    return true;
  }

  // Use this for initialization
  void Start () {
    _levels = new List<Level>();
    loadLevelsFromFile(_levelsPath);
    LoadLevel(2);
  }
  
  // Update is called once per frame
  void Update () {
    
  }
}
