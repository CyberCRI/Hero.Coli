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
  public float scale;

  private static IDictionary<BioBrick.Type, IDictionary<string, string>> BioBrickTextureNames = new Dictionary<BioBrick.Type, IDictionary<string, string>>()
  {
    {BioBrick.Type.PROMOTER, new Dictionary<string, string>(){
        {"PRCONS", "b_PRCONS"},
        {"PRLACI", "b_PRLACI"},
        {"PRTETR", "b_PRTETR"}
      }
    },
    {BioBrick.Type.RBS, new Dictionary<string, string>(){
        {"RBS1", "b_RBS1"},
        {"RBS2", "b_RBS2"}
      }
    },
    {BioBrick.Type.GENE, new Dictionary<string, string>(){
        {"FLUO1", "b_FLUO1"},
        //{"FLUO2", "gfp"},
        {"MOV", "b_MOV"},
        {"AMPR", "b_AMPR"}
        //{"REPR1", "repr1"},
        //{"REPR2", "repr2"}
      }
    },
    {BioBrick.Type.TERMINATOR, new Dictionary<string, string>(){
        {"DTER", "b_DTER"}
      }
    }
  };

  private static IDictionary<string, string> DeviceTextureNames = new Dictionary<string, string>()
  {
    //{"Ampicillin resistance (med)", "ampR_m"},
    {"Green fluorescence (low)", "d_cons-gfp-low"},
    {"Green fluorescence (med)", "d_cons-gfp-med"},
    {"Hyperflagellation (low)", "d_cons-speed-low"},
    {"Hyperflagellation (med)", "d_cons-speed-med"},
    //{"Regulated ampicillin resistance (med)", "ampR_m"},
    {"Regulated green fluorescence (low)", "d_plac-gfp-low"},
    {"Regulated green fluorescence (med)", "d_plac-gfp-med"},
    {"Regulated hyperflagellation (low)", "d_plac-speed-low"},
    {"Regulated hyperflagellation (med)", "d_plac-speed-low"}
  };

  public static void tooltip(bool isOver, Device device, Vector3 pos)
  {
    Logger.Log("TooltipManager::tooltip device", Logger.Level.DEBUG);
    if(!isOver || (null == device))
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
    Rect rectout = _instance.tooltipSprite.GetAtlasSprite().outer;
    _instance.tooltipSprite.gameObject.transform.localScale = new Vector3(_instance.scale*rectout.width, _instance.scale*rectout.height, 1f);
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
