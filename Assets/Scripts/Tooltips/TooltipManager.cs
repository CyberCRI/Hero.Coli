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

  private UISprite _backgroundSprite;
  private UILabel _titleLabel;
  private UILabel _typeLabel;
  private UILabel _subtitleLabel;
  private UISprite _illustrationSprite;
  private UILabel _customFieldLabel;
  private UILabel _customValueLabel;
  private UILabel _lengthValueLabel;
  private UILabel _referenceValueLabel;
  private UILabel _energyConsumptionValueLabel;
  private UILabel _explanationLabel;


    private static string _tooltipPrefix     = "TOOLTIP.";
    private static string _titleSuffix       = ".TITLE";
    private static string _subtitleSuffix    = ".SUBTITLE";
    private static string _customFieldSuffix = ".CUSTOMFIELD";
    private static string _customValueSuffix = ".CUSTOMVALUE";
    private static string _lengthSuffix      = ".LENGTH";
    private static string _referenceSuffix   = ".REFERENCE";
    private static string _energySuffix      = ".ENERGYCONSUMPTION";
    private static string _explanationSuffix = ".EXPLANATION";

  //public GameObject _tooltipPanel;
  private UIPanel _tooltipPanel;
  public TooltipPanel bioBrickTooltipPanel;
  public TooltipPanel deviceTooltipPanel;

  public string deviceBackground;
  public string bioBrickBackground;

  public static string getDeviceBackground()
  {
    return _instance.deviceBackground;
  }
  public static string getBioBrickBackground()
  {
    return _instance.bioBrickBackground;
  }

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

  public enum TooltipType {
    BIOBRICK,
    DEVICE
  }

  public Camera uiCamera;

  private static void setVarsFromTooltipPanel(TooltipType type)
  {
    TooltipPanel panel;

    switch(type)
    {
      case TooltipType.BIOBRICK:
        panel = _instance.bioBrickTooltipPanel;
        break;
      case TooltipType.DEVICE:
        panel = _instance.deviceTooltipPanel;
        break;
      default:
        panel = _instance.bioBrickTooltipPanel;
        break;
    }
		
    _instance._tooltipPanel = panel.gameObject.GetComponent<UIPanel>();

    _instance._titleLabel = panel.titleLabel;
    _instance._typeLabel = panel.typeLabel;
    _instance._subtitleLabel = panel.subtitleLabel;
    _instance._illustrationSprite = panel.illustrationSprite;
    _instance._customFieldLabel = panel.customFieldLabel;
    _instance._customValueLabel = panel.customValueLabel;
    _instance._lengthValueLabel = panel.lengthValueLabel;
    _instance._referenceValueLabel = panel.referenceValueLabel;
    _instance._energyConsumptionValueLabel = panel.energyConsumptionValueLabel;
    _instance._explanationLabel = panel.explanationLabel;
    _instance._backgroundSprite = panel.backgroundSprite;
  }

  public static bool displayTooltip()
  {
    if(_instance && _instance._tooltipPanel && _instance._tooltipPanel.gameObject)
    {
      _instance._tooltipPanel.gameObject.SetActive(false);
      return true;
    }
    else
    {
      return false;
    }
  }

  public static bool displayTooltip(bool isOver, Device device, Vector3 pos)
  {
    string code = (null == device)?null:_devicePrefix+device.getInternalName();
    return displayTooltip(isOver, code, pos);
  }

  public static bool displayTooltip(bool isOver, BioBrick brick, Vector3 pos)
  {
    string code = (null == brick)?null:_bioBrickPrefix+brick.getName();
    return displayTooltip(isOver, code, pos);
  }

  private static bool displayTooltip(bool isOver, string code, Vector3 pos)
  {
    if(!isOver || (null == code))
    {
      return displayTooltip();
    }
    else
    {
      if(fillInFieldsFromCode(code))
      {
        _instance._tooltipPanel.gameObject.SetActive(true);

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
    TooltipInfo info = produceTooltipInfo(code);

    if(null != info)
    {
      setVarsFromTooltipPanel(info._tooltipType);

      _instance._backgroundSprite.spriteName   = info._background;
      _instance._titleLabel.text               = info._title;
      _instance._typeLabel.text                = info._type;
      _instance._subtitleLabel.text            = info._subtitle;

      if(null != _instance._illustrationSprite)
        _instance._illustrationSprite.spriteName = info._illustration;

      if((null != _instance._customFieldLabel) && (null != _instance._customValueLabel))
      {
        _instance._customFieldLabel.text = info._customField;
        _instance._customValueLabel.text = info._customValue;
      }

      _instance._lengthValueLabel.text              = info._length;
      _instance._referenceValueLabel.text           = info._reference;

      if(null != _instance._energyConsumptionValueLabel)
        _instance._energyConsumptionValueLabel.text   = info._energyConsumption;

      _instance._explanationLabel.text         = info._explanation;

      return true;
    }
    else
    {
      return false;
    }
  }

  private static TooltipInfo produceTooltipInfo(string code)
  {
    TooltipInfo retrieved = retrieveFromDico(code);
    return localize(retrieved);
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

  private static TooltipInfo localize(TooltipInfo coded)
  { 
    //TODO test on type
    //string _code;
    //string _background;

    //string _type;
    //TooltipManager.TooltipType _tooltipType;
    //string _illustration;

        if(null != coded && !string.IsNullOrEmpty(coded._code)) {
            string root = _tooltipPrefix+coded._code.ToUpper().Replace(' ','_');

            coded._title = Localization.Localize(root+_titleSuffix);
            coded._subtitle = Localization.Localize(root+_subtitleSuffix);            
            coded._customField = localizeIfExists(root+_customFieldSuffix);
            coded._customValue = localizeIfExists(root+_customValueSuffix);    
            coded._length = Localization.Localize(root+_lengthSuffix);
            coded._reference = Localization.Localize(root+_referenceSuffix);    
            coded._energyConsumption = localizeIfExists(root+_energySuffix);
            coded._explanation = Localization.Localize(root+_explanationSuffix);
        }

        return coded;
  }

  private static string localizeIfExists(string code)
  {
        //TODO get rid of warnings from Localization
    string localization = Localization.Localize(code);
    string res = localization == code ? "" : localization;
    return res;
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
    float xShift, yShift;

    float xScale = _instance._backgroundSprite.transform.localScale.x
      , yScale = _instance._backgroundSprite.transform.localScale.y;

    xShift = xScale/2;
    yShift = yScale/2;

    switch(quadrant)
    {
      case Quadrant.TOP_LEFT:
        yShift = -yShift;
        break;
      case Quadrant.TOP_RIGHT:
        xShift = -xShift;
        yShift = -yShift;
        break;
      case Quadrant.BOTTOM_LEFT:
        break;
      case Quadrant.BOTTOM_RIGHT:
        xShift = -xShift;
        break;
      default:
        Logger.Log("TooltipManager::setPosition default case", Logger.Level.WARN);
        break;
    }
    _instance._tooltipPanel.transform.position = new Vector3(pos.x, pos.y, pos.z);
    Vector3 localPos2 = _instance._tooltipPanel.transform.localPosition;
    _instance._tooltipPanel.transform.localPosition = new Vector3(localPos2.x + xShift, localPos2.y + yShift, -0.9f);
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

  void Start()
  {
    bioBrickTooltipPanel.gameObject.SetActive(false);
    deviceTooltipPanel.gameObject.SetActive(false);
  }
}