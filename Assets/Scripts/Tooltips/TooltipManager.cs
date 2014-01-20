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
    loadDataIntoDico(inputFiles, _loadedInfoWindows);
  }
  ////////////////////////////////////////////////////////////////////////////////////////////


  public string[] inputFiles;

  public UILabel titleLabel;
  public UILabel typeLabel;
  public UILabel subtitleLabel;
  public UILabel lengthValue;
  public UILabel referenceValue;
  public UILabel explanationLabel;

  //public GameObject tooltipPanel;
  public UIPanel tooltipPanel;
  public UISprite tooltipSprite;

  public GameStateController gameStateController;

  private Dictionary<string, TooltipInfo> _loadedInfoWindows = new Dictionary<string, TooltipInfo>();
  private static string _bioBrickPrefix = "b_";
  private static string _devicePrefix = "d_";

  public enum Quadrant {
    TOP_LEFT,
    TOP_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_RIGHT
  }

  public Camera uiCamera;
  //public float scale;

  /*
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
  */

  public static bool displayTooltip()
  {
    Logger.Log("TooltipManager::displayTooltip()", Logger.Level.TEMP);
    _instance.tooltipPanel.gameObject.SetActive(false);
    return true;
  }

  public static bool displayTooltip(bool isOver, Device device, Vector3 pos)
  {
    Logger.Log("TooltipManager::displayTooltip("+device+")", Logger.Level.TEMP);
    string code = (null == device)?null:_devicePrefix+device.getName();
    return displayTooltip(isOver, code, pos);
  }

  public static bool displayTooltip(bool isOver, BioBrick brick, Vector3 pos)
  {
    Logger.Log("TooltipManager::displayTooltip("+brick+")", Logger.Level.TEMP);
    string code = (null == brick)?null:_bioBrickPrefix+brick.getName();
    return displayTooltip(isOver, code, pos);
  }

  private static bool displayTooltip(bool isOver, string code, Vector3 pos)
  {
    Logger.Log("TooltipManager::displayTooltip("+code+")", Logger.Level.TEMP);
    if(!isOver || (null == code))
    {
      Logger.Log("TooltipManager::displayTooltip("+code+") (!isOver || (null == code))", Logger.Level.TEMP);
      return displayTooltip();
    }
    else
    {
      Logger.Log("TooltipManager::displayTooltip("+code+") !(!isOver || (null == code))", Logger.Level.TEMP);
      if(fillInFieldsFromCode(code))
      {
        Logger.Log("TooltipManager::displayTooltip("+code+") fillInFieldsFromCode worked", Logger.Level.TEMP);
        _instance.tooltipPanel.gameObject.SetActive(true);

        //Rect rectout = _instance.tooltipPanel.GetAtlasSprite().outer;
        //_instance.tooltipPanel.gameObject.transform.localScale = new Vector3(_instance.scale*rectout.width, _instance.scale*rectout.height, 1f);

        setPosition(pos);
        return true;
      }
      else
      {
        Logger.Log("TooltipManager::displayTooltip("+code+") failed", Logger.Level.WARN);
        return false;
      }
    }
  }

  private static bool fillInFieldsFromCode(string code)
  {

    TooltipInfo info = retrieveFromDico(code);

    if(null != info)
    {
      _instance.titleLabel.text          = info._title;
      _instance.typeLabel.text           = info._type;
      _instance.subtitleLabel.text       = info._subtitle;
      _instance.tooltipSprite.spriteName = info._texture;
      _instance.lengthValue.text         = info._length;
      _instance.referenceValue.text      = info._reference;
      _instance.explanationLabel.text    = info._explanation;

      return true;
    }
    else
    {
      return false;
    }
  }

  private static TooltipInfo retrieveFromDico(string code)
  {
    TooltipInfo info;
    if(!_instance._loadedInfoWindows.TryGetValue(code, out info))
    {
      Logger.Log("TooltipManager::retrieveFromDico("+code+") failed", Logger.Level.WARN);
      info = null;
    }
    return info;
  }

  private void loadDataIntoDico(string[] inputFiles, Dictionary<string, TooltipInfo> dico)
  {

    TooltipLoader tLoader = new TooltipLoader();

    string loadedFiles = "";

    foreach (string file in inputFiles) {
      foreach (TooltipInfo info in tLoader.loadInfoFromFile(file)) {
        dico.Add(info._code, info);
      }
      if(!string.IsNullOrEmpty(loadedFiles)) {
        loadedFiles += ", ";
      }
      loadedFiles += file;
    }

    Logger.Log("TooltipManager::loadDataIntoDico loaded "+loadedFiles, Logger.Level.DEBUG);
  }

  private static void setPosition(Vector3 pos)
  {
    Quadrant quadrant = getQuadrant(pos);
    switch(quadrant)
    {
      case Quadrant.TOP_LEFT:
        //TODO
      //_instance.tooltipPanel.
        //_instance.tooltipPanel.pivot = UIWidget.Pivot.TopLeft;
        break;
      case Quadrant.TOP_RIGHT:
        //_instance.tooltipPanel.pivot = UIWidget.Pivot.TopRight;
        break;
      case Quadrant.BOTTOM_LEFT:
        //_instance.tooltipPanel.pivot = UIWidget.Pivot.BottomLeft;
        break;
      case Quadrant.BOTTOM_RIGHT:
        //_instance.tooltipPanel.pivot = UIWidget.Pivot.BottomRight;
        break;
    }
    _instance.tooltipPanel.transform.position = new Vector3(pos.x, pos.y, _instance.tooltipPanel.transform.position.z);
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