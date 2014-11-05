using UnityEngine;
using System.Collections;
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

  public UILabel titleLabel;
  public UILabel subtitleLabel;
  public UILabel explanationLabel;
  public UILabel bottomLabel;
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
    GOTOCRAFT
  }

  private Dictionary<string, NextAction> _actions = new Dictionary<string, NextAction>(){
    {InfoWindowXMLTags.WORLD, NextAction.GOTOWORLD},
    {InfoWindowXMLTags.EQUIP, NextAction.GOTOEQUIP},
    {InfoWindowXMLTags.CRAFT, NextAction.GOTOCRAFT}
  };

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

      _instance.titleLabel.text       = Localization.Localize(generic+_genericTitle);
      _instance.subtitleLabel.text    = Localization.Localize(generic+_genericSubtitle);
      _instance.infoSprite.spriteName = info._texture;
      _instance.explanationLabel.text = Localization.Localize(generic+_genericExplanation);
      _instance.bottomLabel.text      = Localization.Localize(generic+_genericBottom);
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

  public static void next()
  {
    ModalManager.unsetModal();
    switch(_instance.nextAction)
    {
      case NextAction.GOTOWORLD:
        Logger.Log("InfoWindowManager::next GOTOWORLD", Logger.Level.DEBUG);
        _instance.gameStateController.tryUnlockPause();
        break;
      case NextAction.GOTOEQUIP:
        Logger.Log("InfoWindowManager::next GOTOEQUIP", Logger.Level.DEBUG);
        GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen2);
        break;
      case NextAction.GOTOCRAFT:
        Logger.Log("InfoWindowManager::next GOTOCRAFT", Logger.Level.DEBUG);
        GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen3);
        break;
      default:
        Logger.Log("InfoWindowManager::next GOTOWORLD", Logger.Level.DEBUG);
        _instance.gameStateController.tryUnlockPause();
        break;
    }
  }

  public static NextAction getFromString(string next)
  {
    NextAction action;
    _instance._actions.TryGetValue(next, out action);
    return action;
  }
}

