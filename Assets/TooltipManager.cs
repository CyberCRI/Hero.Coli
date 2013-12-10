using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipManager : MonoBehaviour {

  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "TooltipManager";
  private static TooltipManager _instance;
  public static TooltipManager get() {
    if(_instance == null) {
      Logger.Log("TooltipManager::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<TooltipManager>();
    }
    return _instance;
  }
  void Awake()
  {
    Logger.Log("TooltipManager::Awake", Logger.Level.DEBUG);
    _instance = this;
  }
  ////////////////////////////////////////////////////////////////////////////////////////////

  public enum Quadrant {
    TOP_LEFT,
    TOP_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_RIGHT
  }

  public Camera uiCamera;
  public UISprite tooltipSprite;
  private static IDictionary<BioBrick.Type, IDictionary<string, string>> BioBrickTextureNames = new Dictionary<BioBrick.Type, IDictionary<string, string>>()
  {
    {BioBrick.Type.PROMOTER, new Dictionary<string, string>(){
        {"pCons", "pCons"},
        {"pLac", "pLac"},
        {"pTet", "pTet"}
      }
    },
    {BioBrick.Type.RBS, new Dictionary<string, string>(){
        {"RBS low", "rbs_l"},
        {"RBS very low", "rbs_vl"}
      }
    },
    {BioBrick.Type.GENE, new Dictionary<string, string>(){
        {"Z", "ampR"},
        //{"ampR", "ampR"},
        {"GFP", "gfp"},
        {"MOV4", "hyperflagellation"}
        //{"FLhDC", "hyperflagellation"},
      }
    },
    {BioBrick.Type.TERMINATOR, new Dictionary<string, string>(){
        {"Terminator", "terminator"}
      }
    }
  };

  private static IDictionary<string, string> DeviceTextureNames = new Dictionary<string, string>()
  {
    {"Ampicillin resistance (med)", "ampR_m"},
    {"Green fluorescence (med)", "gfp_m"},
    {"Hyperflagellation (low)", "speed_l"},
    {"Hyperflagellation (med)", "speed_m"}
  };

  public static void tooltip(bool isOver, Device device, Vector3 pos)
  {
    Logger.Log("TooltipManager::tooltip device", Logger.Level.DEBUG);
    if(!isOver)
    {
      tooltip();
    }
    else
    {
      string spriteName;
      if(DeviceTextureNames.TryGetValue(device.getName(), out spriteName))
      {
        setSpriteNameAndSetActive(spriteName);
      }
      else
      {
        Logger.Log("TooltipManager::tooltip couldn't find texture for device name "+device.getName(), Logger.Level.WARN);
      }
      setPosition(pos);
    }
  }

  public static void tooltip()
  {
    Logger.Log("TooltipManager::tooltip()", Logger.Level.DEBUG);
    _instance.tooltipSprite.gameObject.SetActive(false);
  }

  public static void tooltip(bool isOver, BioBrick brick, Vector3 pos)
  {
    Logger.Log("TooltipManager::tooltip brick", Logger.Level.DEBUG);
    if(!isOver)
    {
      tooltip();
    }
    else
    {
      IDictionary<string, string> dico;
      string spriteName;
      if(BioBrickTextureNames.TryGetValue(brick.getType(), out dico))
      {
        if(dico.TryGetValue(brick.getName(), out spriteName))
        {
          setSpriteNameAndSetActive(spriteName);
        }
        else
        {
          Logger.Log("TooltipManager::tooltip couldn't find texture for brick name "+brick.getName(), Logger.Level.WARN);
        }
      }
      else
      {
        Logger.Log("TooltipManager::tooltip couldn't find dico for brick type "+brick.getType(), Logger.Level.WARN);
      }
      setPosition(pos);
    }
  }

  private static void setSpriteNameAndSetActive(string spriteName)
  {
    if(null == spriteName) return;
    _instance.tooltipSprite.spriteName = spriteName;
    _instance.tooltipSprite.gameObject.SetActive(true);
  }

  private static void setPosition(Vector3 pos)
  {
    Quadrant quadrant = getQuadrant(pos);
    switch(quadrant)
    {
      case Quadrant.TOP_LEFT:
        _instance.tooltipSprite.pivot = UIWidget.Pivot.TopLeft;
        break;
      case Quadrant.TOP_RIGHT:
        _instance.tooltipSprite.pivot = UIWidget.Pivot.TopRight;
        break;
      case Quadrant.BOTTOM_LEFT:
        _instance.tooltipSprite.pivot = UIWidget.Pivot.BottomLeft;
        break;
      case Quadrant.BOTTOM_RIGHT:
        _instance.tooltipSprite.pivot = UIWidget.Pivot.BottomRight;
        break;
    }
    _instance.tooltipSprite.transform.position = new Vector3(pos.x, pos.y, _instance.tooltipSprite.transform.position.z);
  }

  private static Quadrant getQuadrant(Vector3 pos)
  {
    Vector3 screenPos = _instance.uiCamera.WorldToScreenPoint(pos);
    bool top = screenPos.y > _instance.uiCamera.pixelHeight/2;
    bool left = screenPos.x < _instance.uiCamera.pixelWidth/2;

    if (top)
    {
      if (left)
        return Quadrant.TOP_LEFT;
      else
        return Quadrant.TOP_RIGHT;
    }
    else
    {
      if (left)
        return Quadrant.BOTTOM_LEFT;
      else
        return Quadrant.BOTTOM_RIGHT;
    }
  }
}
