using UnityEngine;
using System.Collections.Generic;

//TODO: merge with ModalManager
public class InfoWindowManager : MonoBehaviour {

  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "InfoWindowManager";
  private static InfoWindowManager _instance;
  public static InfoWindowManager get() {
    if(_instance == null) {
      Logger.Log("InfoWindowManager::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<InfoWindowManager>();
    }
    return _instance;
  }
  void Awake()
  {
    Logger.Log("InfoWindowManager::Awake", Logger.Level.DEBUG);
    _instance = this;
    loadDataIntoDico(inputFiles, _loadedInfoWindows);
  }
  ////////////////////////////////////////////////////////////////////////////////////////////

  public string[] inputFiles;

  public UILocalize titleLabel;
  public UILocalize subtitleLabel;
  public UILocalize explanationLabel;
  public UILocalize bottomLabel;
  public NextAction nextAction;

  public GameObject infoPanel;
  public UISprite infoSprite;

  public GameStateController gameStateController;

  private Dictionary<string, StandardInfoWindowInfo> _loadedInfoWindows = new Dictionary<string, StandardInfoWindowInfo>();
  private static string _genericPrefix = "INFO.";
  private static string _genericTitle = ".TITLE";
  private static string _genericSubtitle = ".SUBTITLE";
  private static string _genericExplanation = ".EXPLANATION";
  private static string _genericBottom = ".BOTTOM";

  public enum NextAction
  {
    GOTOWORLD,
    GOTOEQUIP,
    GOTOCRAFT,
    GOTOCRAFTTUTO,
    GOTOCRAFTTUTO2
  }

  private Dictionary<string, NextAction> _actions = new Dictionary<string, NextAction>(){
    {InfoWindowXMLTags.WORLD, NextAction.GOTOWORLD},
    {InfoWindowXMLTags.EQUIP, NextAction.GOTOEQUIP},
    {InfoWindowXMLTags.CRAFT, NextAction.GOTOCRAFT},
    {InfoWindowXMLTags.CRAFTTUTO, NextAction.GOTOCRAFTTUTO},
    {InfoWindowXMLTags.CRAFTTUTO2, NextAction.GOTOCRAFTTUTO2}
  };

    public static bool hasActivePanel()
    {
        return _instance.infoPanel.activeInHierarchy;
    }

  public static bool displayInfoWindow(string code)
  {
    if(fillInFieldsFromCode(code))
    {
      ModalManager.setModal(_instance.infoPanel);
      return true;
    }
    else
    {
      Logger.Log("InfoWindowManager::displayInfoWindow("+code+") failed", Logger.Level.WARN);
      return false;
    }
  }

  private static bool fillInFieldsFromCode(string code)
  {

    StandardInfoWindowInfo info = retrieveFromDico(code);

    if(null != info)
    {
      string generic = _genericPrefix+code.ToUpper();

      _instance.titleLabel.key        = generic+_genericTitle;
      _instance.subtitleLabel.key     = generic+_genericSubtitle;
      _instance.infoSprite.spriteName = info._texture;
      _instance.explanationLabel.key  = generic+_genericExplanation;
      _instance.bottomLabel.key       = generic+_genericBottom;
      _instance.nextAction            = getFromString(info._next);

      return true;
    }
    else
    {
      return false;
    }

    // TODO manage onNext
  }

  private static StandardInfoWindowInfo retrieveFromDico(string code)
  {
    StandardInfoWindowInfo info;
    if(!_instance._loadedInfoWindows.TryGetValue(code, out info))
    {
      Logger.Log("InfoWindowManager::retrieveFromDico("+code+") failed", Logger.Level.WARN);
      info = null;
    }
    return info;
  }

  private void loadDataIntoDico(string[] inputFiles, Dictionary<string, StandardInfoWindowInfo> dico)
  {

    InfoWindowLoader iwLoader = new InfoWindowLoader();

    string loadedFiles = "";

    foreach (string file in inputFiles) {
      foreach (StandardInfoWindowInfo info in iwLoader.loadInfoFromFile(file)) {
        dico.Add(info._code, info);
      }
      if(!string.IsNullOrEmpty(loadedFiles)) {
        loadedFiles += ", ";
      }
      loadedFiles += file;
    }

    Logger.Log("InfoWindowManager::loadDataIntoDico loaded "+loadedFiles, Logger.Level.DEBUG);
  }

  public static void next ()
    {
        Logger.Log ("InfoWindowManager::next()", Logger.Level.INFO);
        ModalManager.unsetModal ();
        _instance.gameStateController.tryUnlockPause ();

        switch (_instance.nextAction) {
            case NextAction.GOTOWORLD:
                Logger.Log ("InfoWindowManager::next GOTOWORLD", Logger.Level.DEBUG);
                break;
            case NextAction.GOTOEQUIP:
                Logger.Log ("InfoWindowManager::next GOTOEQUIP", Logger.Level.DEBUG);
                GUITransitioner.get ().GoToScreen (GUITransitioner.GameScreen.screen2);
                break;
            case NextAction.GOTOCRAFT:
                Logger.Log ("InfoWindowManager::next GOTOCRAFT", Logger.Level.DEBUG);
                GUITransitioner.get ().GoToScreen (GUITransitioner.GameScreen.screen3);
                break;
            case NextAction.GOTOCRAFTTUTO:
                Logger.Log ("InfoWindowManager::next GOTOCRAFTTUTO", Logger.Level.DEBUG);
                CraftHint hint = Hero.get().gameObject.GetComponent<CraftHint>();
                if(null == hint)
                {
                    hint = Hero.get().gameObject.AddComponent<CraftHint>();
                }
                hint.bricks++;
                break;
            case NextAction.GOTOCRAFTTUTO2:
                Logger.Log ("InfoWindowManager::next GOTOCRAFTTUTO2", Logger.Level.DEBUG);
                Hero.get().gameObject.AddComponent<RBS2CraftHint>();
                break;
            default:
                Logger.Log ("InfoWindowManager::next GOTOWORLD", Logger.Level.DEBUG);
                break;
        }
    }

  public static NextAction getFromString(string next)
  {
    NextAction action;
    _instance._actions.TryGetValue(next, out action);
    return action;
  }

    public static GameStateTarget manageKeyPresses()
    {
        if(Input.GetKeyDown(KeyCode.Escape)
           || Input.GetKeyDown(KeyCode.Space)
           || Input.GetKeyDown(KeyCode.Return)
           || Input.GetKeyUp (KeyCode.KeypadEnter)
          )
        {
            Logger.Log("InfoWindowManager::manageKeyPresses() - key pressed", Logger.Level.DEBUG);
            next();
            return GameStateTarget.NoTarget;
        }
        return GameStateTarget.NoAction;
    }
}

