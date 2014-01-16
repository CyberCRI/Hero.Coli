using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

  public GameObject nextButton;

  public GameObject infoPanel;
  public UISprite infoSprite;

  public GameStateController gameStateController;

  private Dictionary<string, StandardInfoWindowInfo> _loadedInfoWindows = new Dictionary<string, StandardInfoWindowInfo>();

  public enum NextAction
  {
    GOTOWORLD,
    GOTOEQUIP,
    GOTOCRAFT
  }


  public static bool displayInfoWindow(string code)
  {
    Logger.Log("InfoWindowManager::displayInfoWindow starts", Logger.Level.TEMP);
    if(fillInFieldsFromCode(code))
    {
      _instance.infoPanel.SetActive(true);
      _instance.gameStateController.StateChange(GameState.Pause);
      _instance.gameStateController.dePauseForbidden = true;
      return true;
    }
    else
    {
      Logger.Log("InfoWindowManager::displayInfoWindow("+code+") failed", Logger.Level.TEMP);
      return false;
    }
  }

  private static bool fillInFieldsFromCode(string code)
  {
    Logger.Log("InfoWindowManager::fillInFieldsFromCode("+code+") starts", Logger.Level.TEMP);

    StandardInfoWindowInfo info = retrieveFromDico(code);

    if(null != info)
    {
      _instance.titleLabel.text       = info._title;
      _instance.subtitleLabel.text    = info._subtitle;
      _instance.infoSprite.spriteName = info._texture;
      _instance.explanationLabel.text = info._explanation;
      _instance.bottomLabel.text      = info._bottom;
      _instance.nextAction            = NextAction.GOTOEQUIP;

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
    Logger.Log("InfoWindowManager::retrieveFromDico("+code+") starts", Logger.Level.TEMP);
    StandardInfoWindowInfo info;
    if(!_instance._loadedInfoWindows.TryGetValue(code, out info))
    {
      Logger.Log("InfoWindowManager::retrieveFromDico("+code+") failed", Logger.Level.WARN);
      info = null;
    }
    else
    {
      Logger.Log("InfoWindowManager::retrieveFromDico("+code+") found "+info, Logger.Level.WARN);
    }
    return info;
  }

  private void loadDataIntoDico(string[] inputFiles, Dictionary<string, StandardInfoWindowInfo> dico)
  {
    Logger.Log("InfoWindowManager::loadDataIntoDico() starts", Logger.Level.TEMP);

    InfoWindowLoader iwLoader = new InfoWindowLoader();

    string loadedFiles = "";

    foreach (string file in inputFiles) {
      Logger.Log("InfoWindowManager::loadDataIntoDico loads file "+file, Logger.Level.TEMP);
      foreach (StandardInfoWindowInfo info in iwLoader.loadInfoFromFile(file)) {
        Logger.Log("InfoWindowManager::loadDataIntoDico adds info "+info, Logger.Level.TEMP);
        dico.Add(info._code, info);
        Logger.Log("InfoWindowManager::loadDataIntoDico added info to "
          +Logger.ToString<string, StandardInfoWindowInfo>(dico)
          , Logger.Level.TEMP);
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
    _instance.infoPanel.SetActive(false);
    switch(_instance.nextAction)
    {
      case NextAction.GOTOWORLD:
        Logger.Log("InfoWindowManager::next GOTOWORLD", Logger.Level.TEMP);
        _instance.gameStateController.StateChange(GameState.Game);
        break;
      case NextAction.GOTOEQUIP:
        Logger.Log("InfoWindowManager::next GOTOEQUIP", Logger.Level.TEMP);
        GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen2);
        break;
      case NextAction.GOTOCRAFT:
        Logger.Log("InfoWindowManager::next GOTOCRAFT", Logger.Level.TEMP);
        GUITransitioner.get().GoToScreen(GUITransitioner.GameScreen.screen3);
        break;
      default:
        Logger.Log("InfoWindowManager::next GOTOWORLD", Logger.Level.TEMP);
        _instance.gameStateController.StateChange(GameState.Game);
        break;
    }
  }
}

